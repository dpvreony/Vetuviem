// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Vetuviem.SourceGenerator.Features.Core
{
    /// <summary>
    /// This class was originally from StyleCop. https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Helpers/XmlSyntaxFactory.cs
    /// All credit goes to the StyleCop team.
    /// </summary>
    internal static class XmlSyntaxFactory
    {
        /// <summary>
        /// Initializes static members of the <see cref="XmlSyntaxFactory"/> class.
        /// </summary>
        static XmlSyntaxFactory()
        {
            // Make sure the newline is included. Otherwise the comment and the method will be on the same line.
            InheritdocSyntax = SyntaxFactory.ParseLeadingTrivia(@"/// <inheritdoc />" + Environment.NewLine);
        }

        /// <summary>
        /// Gets a inheritdoc leading trivia comment.
        /// </summary>
        public static SyntaxTriviaList InheritdocSyntax { get; }

        /// <summary>
        /// Generates a summary comment which includes a see also text.
        /// You need to provide a format index for where the see also needs to be inserted.
        /// </summary>
        /// <param name="summaryText">The text with the summary and a format index for the see also.</param>
        /// <param name="seeAlsoText">The uri for the inner see also section.</param>
        /// <returns>The syntax trivia of the comment.</returns>
        public static SyntaxTriviaList GenerateSummarySeeAlsoComment(string summaryText, string seeAlsoText)
        {
            var text = string.Format(CultureInfo.InvariantCulture, summaryText, "<see cref=\"" + seeAlsoText.Replace("<", "{").Replace(">", "}") + "\" />");
            var template = "/// <summary>" + Environment.NewLine +
                           $"/// {text}" + Environment.NewLine +
                           "/// </summary>" + Environment.NewLine;

            return SyntaxFactory.ParseLeadingTrivia(template);
        }

        /// <summary>
        /// Generates a summary comment which includes a see also text.
        /// You need to provide a format index for where the see also needs to be inserted.
        /// It will also provide the ability to set parameters and their comments.
        /// </summary>
        /// <param name="summaryText">The text with the summary and a format index for the see also.</param>
        /// <param name="seeAlsoText">The uri for the inner see also section.</param>
        /// <param name="parameters">Key/Value pairs for the parameters for the comment.</param>
        /// <returns>The syntax trivia of the comment.</returns>
        public static SyntaxTriviaList GenerateSummarySeeAlsoComment(string summaryText, string seeAlsoText, params (string paramName, string paramText)[] parameters)
        {
            var text = string.Format(CultureInfo.InvariantCulture, summaryText, "<see cref=\"" + seeAlsoText.Replace("<", "{").Replace(">", "}") + "\" />");
            var sb = new StringBuilder("/// <summary>")
                .AppendLine()
                .Append("/// ").AppendLine(text)
                .AppendLine("/// </summary>");

            foreach (var parameter in parameters)
            {
                sb.Append("/// <param name=\"").Append(parameter.paramName).Append("\">").Append(parameter.paramText).AppendLine("</param>");
            }

            return SyntaxFactory.ParseLeadingTrivia(sb.ToString());
        }

        /// <summary>
        /// Generates a summary comment.
        /// </summary>
        /// <param name="summaryText">The text of the summary comment.</param>
        /// <returns>The syntax trivia of the comment.</returns>
        public static SyntaxTriviaList GenerateSummaryComment(string summaryText)
        {
            var template = "/// <summary>" + Environment.NewLine +
                           $"/// {summaryText}" + Environment.NewLine +
                           "/// </summary>" + Environment.NewLine;

            return SyntaxFactory.ParseLeadingTrivia(template);
        }

        /// <summary>
        /// Generates a summary comment with a return statement.
        /// </summary>
        /// <param name="summaryText">The text of the summary comment.</param>
        /// <param name="returnValueText">The text of the return value.</param>
        /// <returns>The syntax trivia of the comment.</returns>
        public static SyntaxTriviaList GenerateSummaryComment(string summaryText, string returnValueText)
        {
            var template = "/// <summary>" + Environment.NewLine +
                           $"/// {summaryText}" + Environment.NewLine +
                           "/// </summary>" + Environment.NewLine +
                           $"/// <returns>{returnValueText}///<returns>" + Environment.NewLine;

            return SyntaxFactory.ParseLeadingTrivia(template);
        }

        /// <summary>
        /// Generates a summary comment with documents for parameters.
        /// </summary>
        /// <param name="summaryText">The text of the summary comment.</param>
        /// <param name="parameters">The key/value text of each parameter.</param>
        /// <returns>The syntax trivia of the comment.</returns>
        public static SyntaxTriviaList GenerateSummaryComment(string summaryText, IEnumerable<(string paramName, string paramText)> parameters)
        {
            var sb = new StringBuilder("/// <summary>")
                .AppendLine()
                .Append("/// ").AppendLine(summaryText)
                .AppendLine("/// </summary>");

            foreach (var parameter in parameters)
            {
                sb.Append("/// <param name=\"").Append(parameter.paramName).Append("\">").Append(parameter.paramText).AppendLine("</param>");
            }

            return SyntaxFactory.ParseLeadingTrivia(sb.ToString());
        }

        /// <summary>
        /// Generates a summary comment with documents for parameters and a return value.
        /// </summary>
        /// <param name="summaryText">The text of the summary comment.</param>
        /// <param name="parameters">The key/value text of each parameter.</param>
        /// <param name="returnValueText">The text of the return value.</param>
        /// <returns>The syntax trivia of the comment.</returns>
        public static SyntaxTriviaList GenerateSummaryComment(string summaryText, IEnumerable<(string paramName, string paramText)> parameters, string returnValueText)
        {
            var sb = new StringBuilder("/// <summary>")
                .AppendLine()
                .Append("/// ").AppendLine(summaryText)
                .AppendLine("/// </summary>");

            foreach (var parameter in parameters)
            {
                sb.Append("/// <param name=\"").Append(parameter.paramName).Append("\">").Append(parameter.paramText).AppendLine("</param>");
            }

            sb.Append("/// <returns>").Append(returnValueText).AppendLine("</returns>");
            return SyntaxFactory.ParseLeadingTrivia(sb.ToString());
        }
    }
}
