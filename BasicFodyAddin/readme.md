[![NuGet Status](http://img.shields.io/nuget/v/BasicFodyAddin.Fody.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/BasicFodyAddin.Fody/)

![Icon](https://raw.githubusercontent.com/Fody/Home/master/BasicFodyAddin/package_icon.png)

This is a simple solution used to illustrate how to [write a Fody addin](/pages/write-an-addin.md).


## Usage

See also [Fody usage](/pages/usage.md).


### NuGet installation

Install the [BasicFodyAddin.Fody NuGet package](https://www.nuget.org/packages/BasicFodyAddin.Fody/) and update the [Fody NuGet package](https://www.nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package BasicFodyAddin.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<BasicFodyAddin/>` to [FodyWeavers.xml](/pages/configuration.md#fodyweaversxml)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <BasicFodyAddin/>
</Weavers>
```


## The moving parts

See [writing an addin](/pages/write-an-addin.md)


## Icon

<a href="https://thenounproject.com/term/lego/16919/" target="_blank">Lego</a> designed by <a href="https://thenounproject.com/timur.zima/" target="_blank">Timur Zima</a> from The Noun Project
