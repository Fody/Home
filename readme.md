
# <img src="https://github.com/Fody/Home/raw/master/images/fody.png" height="40px"> Fody

The Home repository is the starting point for people to learn about Fody, the project.

Fody is an extensible tool for weaving .net assemblies. It enables the manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to eliminate that plumbing code through an extensible add-in model. This technique is very powerful, for example you can turn simple properties into full [INotifyPropertyChanged implementations](https://github.com/Fody/PropertyChanged), add [checks for null arguments](https://github.com/Fody/NullGuard), add [Method Timings](https://github.com/Fody/MethodTimer), even [make all your string comparisons case insensitive](https://github.com/Fody/Caseless).



<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

Fody requires significant effort to maintain. As such it relies on financial contributions from the community and sponsorship to ensure its long term viability. **It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/order/3059).**

[Go to licensing FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.


### Platinum Sponsors

Support this project by [becoming a Platinum Sponsor](https://opencollective.com/fody/order/7089). A banner with your company logo will be added here with a link to your website. A "Sponsored by" text and link will be added to the description of the NuGet Package for the life of your sponsorship. You also get 1 hour of remote support per month.

<!--
<a href="https://opencollective.com/fody/tiers/platinum/0/website"><img src="https://opencollective.com/fody/tiers/platinum/0/avatar.svg" height="100px"></a>
-->


### Gold Sponsors

Support this project by [becoming a Gold Sponsor](https://opencollective.com/fody/order/7088). A large company logo will be added here with a link to your website.

<a href="https://www.postsharp.net?utm_source=fody&utm_medium=referral"><img alt="PostSharp" src="https://raw.githubusercontent.com/Fody/Home/master/images/postsharp.png"></a>


### Silver Sponsors

Support this project by [becoming a Silver Sponsor](https://opencollective.com/fody/order/7086). A medium company logo will be added here with a link to your website.

<a href="https://particular.net/"><img alt="Particular Software" width="200px" src="https://raw.githubusercontent.com/Fody/Home/master/images/particular.svg?sanitize=true"></a>


### Bronze Sponsors

Support this project by [becoming a Bronze Sponsor](https://opencollective.com/fody/order/7085). The company avatar will show up here with a link to your OpenCollective Profile.


<a href="https://opencollective.com/fody/tiers/bronze/0/website"><img src="https://opencollective.com/fody/tiers/bronze/0/avatar.svg?avatarHeight=100"></a>


### Patrons and sponsors

Thanks to all the backers and sponsors! Support this project by [becoming a patron](https://opencollective.com/fody/order/3059).

<a href="https://opencollective.com/fody#contributors"><img src="https://opencollective.com/fody/sponsor.svg?width=890&avatarHeight=50&button=false"><img src="https://opencollective.com/fody/backer.svg?width=890&avatarHeight=50&button=false"></a>



<!--- EndOpenCollectiveBackers -->

<a href="#" id="endofbacking"></a>

## Main Fody code repository

The codebase of core Fody engine located at https://github.com/Fody/Fody.


## Documentation and Further Learning

  * [Licensing/Patron FAQ](pages/licensing-patron-faq.md)<br>
    **It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/order/3059).** See [Licensing/Patron FAQ](pages/licensing-patron-faq.md) for more information.
  * [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin)<br>
    A simple project meant to illustrate how to build an addin.
  * [FodyAddinSamples](https://github.com/Fody/FodyAddinSamples)<br>
    A GitHub repo that contains a working sample of every Fody addin.
  * [List of Fody weavers/addins](pages/addins.md)
  * [Donations](pages/donations.md)<br>
    Every month the Fody project will donate a portion of funding raised to a charity or other cause.
  * [Fody Project Configuration Manager](https://github.com/tom-englert/ProjectConfigurationManager/wiki/6.-Fody)<br>
    Provides an interactive tool that can support configuring weavers, which is especially helpful in solutions with many projects.


## Naming

The name "Fody" comes from the small birds that belong to the weaver family [Ploceidae](http://en.wikipedia.org/wiki/Fody).


## Credits

Thanks to the following

 * [Jb Evain](https://github.com/jbevain) for the use of [Mono Cecil](https://github.com/jbevain/cecil)
 * [GitHub](https://github.com/) for project hosting
 * [JetBrains](https://www.jetbrains.com/resharper/) for the generous donation of [ReSharper](https://www.jetbrains.com/resharper/) licenses.
 * [xUnit](https://xunit.github.io/)
 * [AppVeyor](https://www.appveyor.com/) and [Travis CI](https://travis-ci.org/) for build and CI infrastructure
 * [NuGet](https://www.nuget.org/) for package delivery
 * [The Noun Project](http://thenounproject.com) for the <a href="http://thenounproject.com/noun/bird/#icon-No6726">Bird</a> icon designed by <a href="http://thenounproject.com/MARCOHS">Marco Hernandez</a>
