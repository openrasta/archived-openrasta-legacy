// Script# Framework
// Copyright (c) 2007, Nikhil Kothari. All Rights Reserved.
// http://projects.nikhilk.net
//

function FileSystemObject() {
    try {
        var fso = new ActiveXObject('Scripting.FileSystemObject');
        return fso;
    }
    catch (ex) {
    }

    return null;
}
