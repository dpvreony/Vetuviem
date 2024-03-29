# Vetuviem
Vetuvium is a toolkit to support View to View Model binding (MVVM -> V2VM -> Ve-Tu-Viem) aimed at offering a structure to get more re-usability out of ReactiveUI.

## Mission Statement
* To give a mechanism to reduce the amount of boiler plate code being produced, by allowing some of the ReactiveUI specific logic to be hidden away
* Allow the developer to think along the lines of standard behaviours for controls by offering a way to produce re-usable behaviours through a class and\or function design pattern
* Allow the developer to focus on what matters on the ViewModel
* Reduce the cognitive load by
  * Removing the risk of misusing 1 way or 2 way binding
  * Remove the need for the user to think about having to cater for Bind vs BindCommand
* Offer a structure that allows for more work to be done potentially with Source Generators to reduce reflection and improve the build time developer experience.

## Current Status

This is currently a proof of concept alpha. For understanding of the design reasoning please see https://www.dpvreony.com/articles/designing-vetuviem/

## Nuget Packages

[VetuviemCoreNuget]: https://www.nuget.org/packages/Vetuviem.Core/
[VetuviemCoreBadge]: https://img.shields.io/nuget/v/Vetuviem.Core.svg
[VetuviemSourceGeneratorNuGet]: https://www.nuget.org/packages/Vetuviem.SourceGenerator/
[VetuviemSourceGeneratorBadge]: https://img.shields.io/nuget/v/Vetuviem.SourceGenerator.svg

| Purpose | Package | NuGet |
|---------|-------|------|
| Command Line Generation | Coming soon | Coming Soon
| Visual Studio Integration | [Vetuviem.SourceGenerator][VetuviemSourceGeneratorNuGet] | [![VetuviemSourceGeneratorBadge]][VetuviemSourceGeneratorNuGet] |
| Core Functionality | [Vetuviem.Core][VetuviemCoreNuget] | [![VetuviemCoreBadge]][VetuviemCoreNuget] |

## An example

Currently to write binding logic in the codebehind you have to write something similar to this for a single control

``
TODO
``

But what if you have a way to simplify logic and offer a way to even make it reusable without all the boilerplate leg work?

``
TODO
``

## Sponsorship

TODO

## Support

TODO

## Contribute

TODO

