[![NuGet](https://img.shields.io/nuget/v/MonoGame-Textbox.svg?maxAge=2592000)](https://www.nuget.org/packages/MonoGame-Textbox/)
 [![license](https://img.shields.io/github/license/unterrainerinformatik/MonoGame-Textbox.svg?maxAge=2592000)](http://unlicense.org)  [![Twitter Follow](https://img.shields.io/twitter/follow/throbax.svg?style=social&label=Follow&maxAge=2592000)](https://twitter.com/throbax)  

# General

This section contains various useful projects that should help your development-process.  

This section of our GIT repositories is free. You may copy, use or rewrite every single one of its contained projects to your hearts content.  
In order to get help with basic GIT commands you may try [the GIT cheat-sheet][coding] on our [homepage][homepage].  

This repository located on our  [homepage][homepage] is private since this is the master- and release-branch. You may clone it, but it will be read-only.  
If you want to contribute to our repository (push, open pull requests), please use the copy on github located here: [the public github repository][github]  

# ![Icon](https://github.com/UnterrainerInformatik/Monogame-Textbox/raw/master/icon.png)Monogame-Textbox

This is a PCL that provides an editable, multi-language, event-driven textbox for MonoGame that supports selection, special characters, SHIFT-CTRL combinations with cursor-keys and cut/copy/paste.

> **If you like this repo, please don't forget to star it.**
> **Thank you.**



## Getting Started

Clone the repository and run the project `Textbox-Test` as startup project.
Nuget should take care about the rest...

Write, select, delete, cut (ctrl-x), copy (ctrl-c), paste (ctrl-v), press **home**, press **pos1**, keep **shift** pressed while pressing a cursor key to select, try pressing **ctrl** while doing so in order to jump words ahead or back...

![Screenshot](https://github.com/UnterrainerInformatik/MonoGame-Textbox/blob/master/Textbox-Test.png)

### General Information

I generate the SpriteFont used in the test-projects using a `.spritefont` description-file read by the content-pipeline tool because of the insane amount of supported glyphs.

The reason why I don't use the automated build-pipeline for this is that it would require everyone to install the True-Type Font referenced further down this file. You are free to do so, in which case you may as well add the file `Arsenal.spritefont` to your automated build-pipeline of your project.

#### Used Tools

* [Resharper][https://www.jetbrains.com/resharper/]

* [MonoGame](http://www.monogame.net/)

* You can easily get rid of the `Primitives2D.cs` class when you incorporate it in your own project. It's only there to draw the outline of the textbox in the demo.

* The font is called Arsenal and you can find it here [Fontsquirrel - Arsenal](https://www.fontsquirrel.com/fonts/arsenal?q%5Bterm%5D=arsenal&q%5Bsearch_check%5D=Y)

  It has an insane amount of supported languages (and therefore glyphs) and is under the SIL Open Font License v1.10. So it's safe to use in your games.



[homepage]: http://www.unterrainer.info
[coding]: http://www.unterrainer.info/Home/Coding
[github]: https://github.com/UnterrainerInformatik/MonoGame-Textbox