#!/bin/bash
ICON=paracletus.ico
shopt -s globstar
ln -f -s ParacletusConsole/$ICON $ICON
resgen2 ParacletusConsole/ParacletusForm.resx ParacletusConsole/ParacletusConsole.ConsoleForm.resources
resgen2 ParacletusConsole/Properties/Resources.resx ParacletusConsole/Properties/ParacletusConsole.Properties.Resources.resources
dmcs -debug -target:library -out:ParacletusConsole.exe -reference:System.Drawing -reference:System.Windows.Forms -reference:System.Data.DataSetExtensions -reference:Nil -lib:../NilSharp -main:ParacletusConsole.Paracletus -target:exe -linkresource:$ICON -win32icon:$ICON -resource:ParacletusConsole/ParacletusConsole.ConsoleForm.resources -resource:ParacletusConsole/Properties/ParacletusConsole.Properties.Resources.resources **/*.cs
