// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// Helpers for generating roslyn expression syntax.
    /// </summary>
    public static class RoslynGenerationHelpers
    {
        /// <summary>
        /// Creates a parameter list syntax for use in a method invocation.
        /// </summary>
        /// <param name="argCollection">Collection of argument names.</param>
        /// <returns>Parameter List Syntax.</returns>
        public static ParameterListSyntax GetParams(string[] argCollection)
        {
            if (argCollection == null)
            {
                throw new ArgumentNullException(nameof(argCollection));
            }

            var parameters = SyntaxFactory.SeparatedList<ParameterSyntax>();

            foreach (var s in argCollection)
            {
                var node = SyntaxFactory.Parameter(SyntaxFactory.Identifier(s));
                parameters = parameters.Add(node);
            }

            return SyntaxFactory.ParameterList(parameters);
        }

        /// <summary>
        /// Generates an attribute list syntax for use in invocation of a generic method, or reference of a generic class.
        /// </summary>
        /// <param name="attributeArguments">List of argument names.</param>
        /// <returns>Attribute List Syntax.</returns>
        public static AttributeArgumentListSyntax? GetAttributeArgumentListSyntax(IList<string> attributeArguments)
        {
            if (attributeArguments == null || attributeArguments.Count < 1)
            {
                return null;
            }

            var argumentList = SyntaxFactory.AttributeArgumentList();

            foreach (var attributeArgument in attributeArguments)
            {
                var name = SyntaxFactory.ParseName(attributeArgument);
                var attribArgSyntax = SyntaxFactory.AttributeArgument(name);
                argumentList = argumentList.AddArguments(attribArgSyntax);
            }

            return argumentList;
        }

        /// <summary>
        /// Produces the Invocation Syntax for using the nameof operator.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <returns>Roslyn syntax.</returns>
        public static InvocationExpressionSyntax GetNameOfInvocationSyntax(string parameterName)
        {
            var nameOfIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("nameof"));
            var nameOfArgumentList = default(SeparatedSyntaxList<ArgumentSyntax>);
            nameOfArgumentList = nameOfArgumentList.Add(
                SyntaxFactory.Argument(
                    SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(parameterName))));
            return SyntaxFactory.InvocationExpression(nameOfIdentifier, SyntaxFactory.ArgumentList(nameOfArgumentList));
        }

        /// <summary>
        /// Produces the Throw Statement Syntax for an ArgumentNullException.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <returns>Roslyn syntax.</returns>
        public static ThrowStatementSyntax GetThrowArgumentNullExceptionSyntax(string parameterName)
        {
            var nameOfInvocation = GetNameOfInvocationSyntax(parameterName);

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            argsList = argsList.Add(SyntaxFactory.Argument(nameOfInvocation));

            var objectCreationEx = SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.ParseTypeName("global::System.ArgumentNullException"),
                SyntaxFactory.ArgumentList(argsList),
                null);

            return SyntaxFactory.ThrowStatement(objectCreationEx);
        }

        /// <summary>
        /// Produces the Throw Statement Syntax for an NotImplementedException.
        /// </summary>
        /// <returns>Roslyn syntax.</returns>
        public static ThrowStatementSyntax GetThrowNotImplementedExceptionSyntax()
        {
            var objectCreationEx = SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.ParseTypeName(nameof(NotImplementedException)),
                SyntaxFactory.ArgumentList(),
                null);

            return SyntaxFactory.ThrowStatement(objectCreationEx);
        }

        /// <summary>
        /// Produces a null guard check for a parameter.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <returns>Roslyn syntax.</returns>
        public static IfStatementSyntax GetNullGuardCheckSyntax(string parameterName)
        {
            var parameterIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(parameterName));
            var condition = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                parameterIdentifier,
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));

            var throwStatement =
                RoslynGenerationHelpers.GetThrowArgumentNullExceptionSyntax(parameterName);
            return SyntaxFactory.IfStatement(condition, throwStatement);
        }

        /// <summary>
        /// Produces syntax for creating a variable and assigning from executing a method on a field.
        /// </summary>
        /// <param name="variableName">The name of the variable to create.</param>
        /// <param name="fieldName">The name of the field to access.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="args">Collection of args to pass to the method.</param>
        /// <returns>Roslyn Syntax for carrying out the variable declaration.</returns>
        public static LocalDeclarationStatementSyntax GetVariableAssignmentFromMethodOnFieldSyntax(string variableName, string fieldName, string methodName, string[] args)
        {
            var fieldMemberAccess = SyntaxFactory.MemberAccessExpression(
                  SyntaxKind.SimpleMemberAccessExpression,
                  SyntaxFactory.IdentifierName("this"),
                  name: SyntaxFactory.IdentifierName(fieldName));

            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                  SyntaxKind.SimpleMemberAccessExpression,
                  fieldMemberAccess,
                  name: SyntaxFactory.IdentifierName(methodName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            ExpressionSyntax getCommandInvocation = SyntaxFactory.InvocationExpression(getAddCommandInvocation, SyntaxFactory.ArgumentList(argsList));
            getCommandInvocation = GetAwaitedExpressionSyntax(true, getCommandInvocation);

            var equalsValueClause = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.Token(SyntaxKind.EqualsToken),
                getCommandInvocation);
            var variableSyntax = default(SeparatedSyntaxList<VariableDeclaratorSyntax>);
            variableSyntax = variableSyntax.Add(SyntaxFactory.VariableDeclarator(variableName).WithInitializer(equalsValueClause));
            var variableDeclaration =
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("var")), variableSyntax);
            return SyntaxFactory.LocalDeclarationStatement(variableDeclaration);
        }

        /// <summary>
        /// Produces syntax for creating a variable and assigning from accessing a property on a field.
        /// </summary>
        /// <param name="variableName">The name of the variable to create.</param>
        /// <param name="fieldName">The name of the field to access.</param>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>Roslyn Syntax for carrying out the variable declaration.</returns>
        public static LocalDeclarationStatementSyntax GetVariableAssignmentFromVariablePropertyAccessSyntax(
            string variableName,
            string fieldName,
            string propertyName)
        {
            var fieldMemberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("this"),
                name: SyntaxFactory.IdentifierName(fieldName));

            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                fieldMemberAccess,
                name: SyntaxFactory.IdentifierName(propertyName));

            var equalsValueClause = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.Token(SyntaxKind.EqualsToken),
                getAddCommandInvocation);
            var variableSyntax = default(SeparatedSyntaxList<VariableDeclaratorSyntax>);
            variableSyntax = variableSyntax.Add(SyntaxFactory.VariableDeclarator(variableName).WithInitializer(equalsValueClause));
            var variableDeclaration =
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("var")), variableSyntax);
            return SyntaxFactory.LocalDeclarationStatement(variableDeclaration);
        }

        /// <summary>
        /// Produces a null guard check for a parameter.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <param name="returnName">name of the return variable.</param>
        /// <returns>Roslyn syntax.</returns>
        public static IfStatementSyntax GetReturnIfNullSyntax(string parameterName, string returnName)
        {
            var parameterIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(parameterName));
            var condition = SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                parameterIdentifier,
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));

            var returnStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.ParseExpression(returnName));
            return SyntaxFactory.IfStatement(condition, returnStatement);
        }

        /// <summary>
        /// Produces a false check for a parameter.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <param name="returnName">name of the return variable.</param>
        /// <returns>Roslyn syntax.</returns>
        public static IfStatementSyntax GetReturnIfFalseSyntax(string parameterName, string returnName)
        {
            var parameterIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(parameterName));

            var condition = SyntaxFactory.PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                parameterIdentifier);

            var returnStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.ParseExpression(returnName));
            return SyntaxFactory.IfStatement(condition, returnStatement);
        }

        /// <summary>
        /// Produces a null guard check for a parameter.
        /// </summary>
        /// <param name="parameterName">parameter name.</param>
        /// <param name="minimumValue">The minimum value for the check.</param>
        /// <param name="returnName">The name of the return variable.</param>
        /// <returns>Roslyn syntax.</returns>
        public static IfStatementSyntax GetReturnIfLessThanSyntax(string parameterName, int minimumValue, string returnName)
        {
            var parameterIdentifier = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(parameterName));
            var condition = SyntaxFactory.BinaryExpression(
                SyntaxKind.LessThanExpression,
                parameterIdentifier,
                SyntaxFactory.ParseExpression(minimumValue.ToString(NumberFormatInfo.InvariantInfo)));

            var returnStatement = SyntaxFactory.ReturnStatement(SyntaxFactory.ParseExpression(returnName));
            return SyntaxFactory.IfStatement(condition, returnStatement);
        }

        /// <summary>
        /// Gets syntax to create and assign a variable from invoking another variable.
        /// </summary>
        /// <param name="variableToCreate">The name of the variable to create and assign to.</param>
        /// <param name="variableToInvoke">The name of the variable to invoke.</param>
        /// <param name="isAsync">Whether the variable is an async method invocation.</param>
        /// <returns>Statement representing invoking a variable and assigning it to a newly created variable.</returns>
        public static StatementSyntax GetVariableAssignmentFromVariableInvocationSyntax(string variableToCreate, string variableToInvoke, bool isAsync)
        {
            ExpressionSyntax getCommandInvocation = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(variableToInvoke));

            getCommandInvocation = GetAwaitedExpressionSyntax(isAsync, getCommandInvocation);

            var equalsValueClause = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.Token(SyntaxKind.EqualsToken),
                getCommandInvocation);

            var variableSyntax = default(SeparatedSyntaxList<VariableDeclaratorSyntax>);
            variableSyntax = variableSyntax.Add(SyntaxFactory.VariableDeclarator(variableToCreate).WithInitializer(equalsValueClause));

            var variableDeclaration = SyntaxFactory.VariableDeclaration(
                SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("var")),
                variableSyntax);

            return SyntaxFactory.LocalDeclarationStatement(variableDeclaration);
        }

        /// <summary>
        /// Gets syntax to create and assign a variable from invoking another a method on another variable.
        /// </summary>
        /// <param name="variableToCreate">The name of the variable to create and assign to.</param>
        /// <param name="variableToReference">The name of the variable to reference when invoking the member.</param>
        /// <param name="methodToInvoke">The name of the variable to invoke.</param>
        /// <param name="arguments">arguments to pass to the invoked method.</param>
        /// <returns>Statement representing invoking a variable and assigning it to a newly created variable.</returns>
        public static StatementSyntax GetVariableAssignmentFromVariableInvocationSyntax(
            string variableToCreate,
            string variableToReference,
            string methodToInvoke,
            string[] arguments)
        {
            var fieldMemberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(variableToReference),
                SyntaxFactory.IdentifierName(methodToInvoke));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (arguments != null && arguments.Length > 0)
            {
                foreach (var s in arguments)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var argListSyntax = SyntaxFactory.ArgumentList(argsList);

            ExpressionSyntax getCommandInvocation = SyntaxFactory.InvocationExpression(fieldMemberAccess, argListSyntax);

            getCommandInvocation = GetAwaitedExpressionSyntax(true, getCommandInvocation);

            var equalsValueClause = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.Token(SyntaxKind.EqualsToken),
                getCommandInvocation);

            var variableSyntax = default(SeparatedSyntaxList<VariableDeclaratorSyntax>);
            variableSyntax = variableSyntax.Add(SyntaxFactory.VariableDeclarator(variableToCreate)
                .WithInitializer(equalsValueClause));

            var variableDeclaration =
                SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("var")), variableSyntax);
            return SyntaxFactory.LocalDeclarationStatement(variableDeclaration);
        }

        /// <summary>
        /// Gets the syntax for invoking a method on a field and passing in a value.
        /// </summary>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="args">Collection of args to pass to the method.</param>
        /// <param name="isAsync">Whether the variable is an async method invocation.</param>
        /// <returns>Invocation Syntax.</returns>
        public static ExpressionSyntax GetMethodOnClassInvocationSyntax(
            string methodName,
            string[] args,
            bool isAsync)
        {
            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("this"),
                name: SyntaxFactory.IdentifierName(methodName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var getCommandInvocation = SyntaxFactory.InvocationExpression(getAddCommandInvocation, SyntaxFactory.ArgumentList(argsList));

            if (!isAsync)
            {
                return getCommandInvocation;
            }

            var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                getCommandInvocation,
                name: SyntaxFactory.IdentifierName("ConfigureAwait"));

            var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            configureAwaitArgsList =
                configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));
            getCommandInvocation = SyntaxFactory.InvocationExpression(configureAwaitMemberAccessExpression, SyntaxFactory.ArgumentList(configureAwaitArgsList));

            var awaitCommand = SyntaxFactory.AwaitExpression(
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                getCommandInvocation);
            return awaitCommand;
        }

        /// <summary>
        /// Generates an statement for invoking a method on a static class.
        /// </summary>
        /// <param name="className">Fully qualified name of the class.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="nestedSyntax">nested syntax for arguments to be passed into the method.</param>
        /// <param name="isAsync">Whether the method is async.</param>
        /// <returns>Invocation syntax.</returns>
        public static ExpressionSyntax GetStaticMethodInvocationSyntax(
            string className,
            string methodName,
            ExpressionSyntax nestedSyntax,
            bool isAsync)
        {
            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(className),
                name: SyntaxFactory.IdentifierName(methodName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            argsList = argsList.Add(SyntaxFactory.Argument(nestedSyntax));

            var getCommandInvocation = SyntaxFactory.InvocationExpression(getAddCommandInvocation, SyntaxFactory.ArgumentList(argsList));

            if (!isAsync)
            {
                return getCommandInvocation;
            }

            var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                getCommandInvocation,
                name: SyntaxFactory.IdentifierName("ConfigureAwait"));

            var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            configureAwaitArgsList =
                configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));
            getCommandInvocation = SyntaxFactory.InvocationExpression(
                configureAwaitMemberAccessExpression,
                SyntaxFactory.ArgumentList(configureAwaitArgsList));

            var awaitCommand = SyntaxFactory.AwaitExpression(
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                getCommandInvocation);
            return awaitCommand;
        }

        /// <summary>
        /// Gets the syntax for invoking a method on a field and passing in a value.
        /// </summary>
        /// <param name="className">Name of the class to access.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="args">Collection of args to pass to the method.</param>
        /// <param name="isAsync">Whether the variable is an async method invocation.</param>
        /// <returns>Invocation statement.</returns>
        public static ExpressionSyntax GetStaticMethodInvocationSyntax(string className, string methodName, string[] args, bool isAsync)
        {
            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(className),
                name: SyntaxFactory.IdentifierName(methodName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var getCommandInvocation = SyntaxFactory.InvocationExpression(getAddCommandInvocation, SyntaxFactory.ArgumentList(argsList));

            if (!isAsync)
            {
                return getCommandInvocation;
            }

            var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                getCommandInvocation,
                name: SyntaxFactory.IdentifierName("ConfigureAwait"));

            var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            configureAwaitArgsList =
                configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));
            getCommandInvocation = SyntaxFactory.InvocationExpression(
                configureAwaitMemberAccessExpression,
                SyntaxFactory.ArgumentList(configureAwaitArgsList));

            var awaitCommand = SyntaxFactory.AwaitExpression(
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                getCommandInvocation);

            return awaitCommand;
        }

        /// <summary>
        /// Gets the syntax for invoking a static method on the local class and passing in a value.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="args">Collection of args to pass to the method.</param>
        /// <param name="isAsync">Whether the variable is an async method invocation.</param>
        /// <returns>Invocation Statement.</returns>
        public static ExpressionSyntax GetStaticMethodInvocationSyntax(string methodName, string[] args, bool isAsync)
        {
            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var getCommandInvocation = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(methodName), SyntaxFactory.ArgumentList(argsList));

            if (!isAsync)
            {
                return getCommandInvocation;
            }

            var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                getCommandInvocation,
                name: SyntaxFactory.IdentifierName("ConfigureAwait"));

            var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            configureAwaitArgsList =
                configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));
            getCommandInvocation = SyntaxFactory.InvocationExpression(configureAwaitMemberAccessExpression, SyntaxFactory.ArgumentList(configureAwaitArgsList));

            var awaitCommand = SyntaxFactory.AwaitExpression(
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                getCommandInvocation);

            return awaitCommand;
        }

        /// <summary>
        /// Gets the syntax for invoking a method on a field and passing in a value.
        /// </summary>
        /// <param name="fieldName">The name of the field to access.</param>
        /// <param name="methodName">The name of the method on the field.</param>
        /// <param name="args">Collection of args to pass to the method.</param>
        /// <param name="isAsync">Whether the variable is an async method invocation.</param>
        /// <returns>Invocation statement.</returns>
        public static StatementSyntax GetMethodOnFieldInvocationSyntax(
            string fieldName,
            string methodName,
            string[] args,
            bool isAsync)
        {
            var fieldMemberAccess = SyntaxFactory.MemberAccessExpression(
                  SyntaxKind.SimpleMemberAccessExpression,
                  SyntaxFactory.IdentifierName("this"),
                  name: SyntaxFactory.IdentifierName(fieldName));

            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                  SyntaxKind.SimpleMemberAccessExpression,
                  fieldMemberAccess,
                  name: SyntaxFactory.IdentifierName(methodName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);

            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var getCommandInvocation = SyntaxFactory.InvocationExpression(getAddCommandInvocation, SyntaxFactory.ArgumentList(argsList));

            if (!isAsync)
            {
                return SyntaxFactory.ExpressionStatement(getCommandInvocation);
            }

            var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                getCommandInvocation,
                name: SyntaxFactory.IdentifierName("ConfigureAwait"));

            var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            configureAwaitArgsList =
                configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));
            getCommandInvocation = SyntaxFactory.InvocationExpression(configureAwaitMemberAccessExpression, SyntaxFactory.ArgumentList(configureAwaitArgsList));

            var awaitCommand = SyntaxFactory.AwaitExpression(
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                getCommandInvocation);

            return SyntaxFactory.ExpressionStatement(awaitCommand);
        }

        /// <summary>
        /// Gets the syntax for invoking a method on a field and passing in a value.
        /// </summary>
        /// <param name="variableName">Name of the variable to access.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="args">Collection of args to pass to the method.</param>
        /// <param name="isAsync">Whether the variable is an async method invocation.</param>
        /// <returns>Invocation statement.</returns>
        public static StatementSyntax GetMethodOnVariableInvocationSyntax(
            string variableName,
            string methodName,
            string[] args,
            bool isAsync)
        {
            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                  SyntaxKind.SimpleMemberAccessExpression,
                  SyntaxFactory.IdentifierName(variableName),
                  SyntaxFactory.IdentifierName(methodName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var getCommandInvocation = SyntaxFactory.InvocationExpression(getAddCommandInvocation, SyntaxFactory.ArgumentList(argsList));

            if (!isAsync)
            {
                return SyntaxFactory.ExpressionStatement(getCommandInvocation);
            }

            var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                getCommandInvocation,
                name: SyntaxFactory.IdentifierName("ConfigureAwait"));

            var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            configureAwaitArgsList =
                configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));
            getCommandInvocation = SyntaxFactory.InvocationExpression(configureAwaitMemberAccessExpression, SyntaxFactory.ArgumentList(configureAwaitArgsList));

            var awaitCommand = SyntaxFactory.AwaitExpression(
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                getCommandInvocation);

            return SyntaxFactory.ExpressionStatement(awaitCommand);
        }

        /// <summary>
        /// Gets the syntax for invoking a method on a property that is on a variable and passing in a value.
        /// </summary>
        /// <param name="variableName">Name of the variable to access.</param>
        /// <param name="propertyName">Name of the property on the variable to access.</param>
        /// <param name="methodName">The name of the method on the property to invoke.</param>
        /// <param name="args">Collection of args to pass to the method.</param>
        /// <returns>Invocation Statement.</returns>
        public static StatementSyntax GetMethodOnPropertyOfVariableInvocationSyntax(
            string variableName,
            string propertyName,
            string methodName,
            string[] args)
        {
            var variableAccess = SyntaxFactory.MemberAccessExpression(
                  SyntaxKind.SimpleMemberAccessExpression,
                  SyntaxFactory.IdentifierName(variableName),
                  SyntaxFactory.IdentifierName(propertyName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var conditionalAccess = SyntaxFactory.ConditionalAccessExpression(variableAccess, SyntaxFactory.MemberBindingExpression(SyntaxFactory.IdentifierName(methodName)));

            var getCommandInvocation = SyntaxFactory.InvocationExpression(conditionalAccess, SyntaxFactory.ArgumentList(argsList));

            return SyntaxFactory.ExpressionStatement(getCommandInvocation);
        }

        /// <summary>
        /// Generates an invocation for invoking a method on a variable.
        /// </summary>
        /// <param name="variableName">Name of the variable to access.</param>
        /// <param name="methodName">Name of the method on the variable to invoke.</param>
        /// <param name="args">Collection of arguments to pass to the method.</param>
        /// <param name="isAsync">Whether the method is asynchronous.</param>
        /// <returns>Invocation statement.</returns>
        public static ExpressionSyntax GetMethodOnVariableInvocationExpression(
            string variableName,
            string methodName,
            string[] args,
            bool isAsync)
        {
            var getAddCommandInvocation = SyntaxFactory.MemberAccessExpression(
                  SyntaxKind.SimpleMemberAccessExpression,
                  SyntaxFactory.IdentifierName(variableName),
                  SyntaxFactory.IdentifierName(methodName));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var getCommandInvocation = SyntaxFactory.InvocationExpression(getAddCommandInvocation, SyntaxFactory.ArgumentList(argsList));

            if (!isAsync)
            {
                return getCommandInvocation;
            }

            var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                getCommandInvocation,
                name: SyntaxFactory.IdentifierName("ConfigureAwait"));

            var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            configureAwaitArgsList =
                configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));

            getCommandInvocation = SyntaxFactory.InvocationExpression(
                configureAwaitMemberAccessExpression,
                SyntaxFactory.ArgumentList(configureAwaitArgsList));

            var awaitCommand = SyntaxFactory.AwaitExpression(
                SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                getCommandInvocation);

            return awaitCommand;
        }

        /// <summary>
        /// Generates a Fluent API invocation statement.
        /// </summary>
        /// <param name="fluentApiInvocation">Existing fluent API expression to extend.</param>
        /// <param name="name">Name of the method to invoke.</param>
        /// <param name="args">Collection of arguments to pass to the method.</param>
        /// <returns>Invocation expression.</returns>
        public static InvocationExpressionSyntax GetFluentApiChainedInvocationExpression(
            ExpressionSyntax fluentApiInvocation,
            string name,
            string[] args)
        {
            var memberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                fluentApiInvocation,
                SyntaxFactory.IdentifierName(name));

            var argsList = default(SeparatedSyntaxList<ArgumentSyntax>);
            if (args != null && args.Length > 0)
            {
                foreach (var s in args)
                {
                    argsList = argsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseName(s)));
                }
            }

            var invocationExpression = SyntaxFactory.InvocationExpression(memberAccessExpression, SyntaxFactory.ArgumentList(argsList));

            return invocationExpression;
        }

        private static ExpressionSyntax GetAwaitedExpressionSyntax(
            bool isAsync,
            ExpressionSyntax getCommandInvocation)
        {
            if (isAsync)
            {
                var configureAwaitMemberAccessExpression = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    getCommandInvocation,
                    name: SyntaxFactory.IdentifierName("ConfigureAwait"));

                var configureAwaitArgsList = default(SeparatedSyntaxList<ArgumentSyntax>);
                configureAwaitArgsList =
                    configureAwaitArgsList.Add(SyntaxFactory.Argument(SyntaxFactory.ParseExpression("false")));
                getCommandInvocation = SyntaxFactory.InvocationExpression(
                    configureAwaitMemberAccessExpression,
                    SyntaxFactory.ArgumentList(configureAwaitArgsList));

                getCommandInvocation = SyntaxFactory.AwaitExpression(
                    SyntaxFactory.Token(SyntaxKind.AwaitKeyword),
                    getCommandInvocation);
            }

            return getCommandInvocation;
        }
    }
}
