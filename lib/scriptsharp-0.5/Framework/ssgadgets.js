// Script# Gadgets Bootstrapper
// Copyright (c) 2007, Nikhil Kothari. All Rights Reserved.
// http://projects.nikhilk.net
//
var Gadget=System.Gadget;var Sidebar=System.Gadget.Sidebar;var SideShow=System.Gadget.SideShow;var EnvironmentService=System.Environment;var TimeService=System.Time;var SoundService=System.Sound;var ShellService=System.Shell;Debug.assert=function Gadgets$Debug$assert(condition,message){if(!condition){message='Assert failed: '+message;Debug.writeLine(message);Debug._fail(message);}}
Debug.writeLine=function Gadgets$Debug$writeLine(message){System.Debug.outputString(message+'\r\n');}