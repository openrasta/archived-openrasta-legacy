//! Script# RSS Feeds Bootstrapper
//! Copyright (c) 2007, Nikhil Kothari. All Rights Reserved.
//! http://projects.nikhilk.net
//!

function FeedsManager() {
    try {
        var fm = new ActiveXObject('Microsoft.FeedsManager');
        return fm;
    }
    catch (ex) {
    }

    return null;
}
