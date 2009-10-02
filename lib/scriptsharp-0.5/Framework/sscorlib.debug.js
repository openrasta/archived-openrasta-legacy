//! Script# Core Runtime
//! Copyright (c) 2007, Nikhil Kothari. All Rights Reserved.
//! http://projects.nikhilk.net
//!

///////////////////////////////////////////////////////////////////////////////
// Globals

function isUndefined(o) {
    return (o === undefined);
}

function isNull(o) {
    return (o === null);
}

function isNullOrUndefined(o) {
    return (o === null) || (o === undefined);
}

window.__scriptsharp = '0.5.1.0';

function $(id) {
    return document.getElementById(id);
}

document.getElementsBySelector = function getElementsBySelector(cssSelector, root) {
    var all = root ? root.getElementsByTagName('*') : document.getElementsByTagName('*');
    var matches = [];

    // Create a stylesheet we'll use to insert a CSS rule into to do the matching.
    // This is created just once.
    var styleSheet = document.getElementsBySelector.styleSheet;
    if (!styleSheet) {
        var styleSheetNode = document.createElement('style');
        styleSheetNode.type = 'text/css';

        document.getElementsByTagName('head')[0].appendChild(styleSheetNode);
        styleSheet = styleSheetNode.styleSheet || styleSheetNode.sheet;
        
        document.getElementsBySelector.styleSheet = styleSheet;
    }

    if (window.navigator.userAgent.indexOf('MSIE') >= 0) {
        // Add the rule that will be used to match elements
        styleSheet.addRule(cssSelector, 'ssCssMatch:true', 0);

        for (var i = all.length - 1; i >= 0; i--) {
            var element = all[i];

            // Find elements that contain the special css attribute, i.e.
            // the elements that match the specified selector.
            if (element.currentStyle.ssCssMatch) {
                matches[matches.length] = element;
            }
        }

        styleSheet.removeRule(0);
    }
    else {
        var matchValue = document.getElementsBySelector.matchValue;
        if (!matchValue) {
            matchValue = (window.navigator.userAgent.indexOf('Opera') >= 0) ? '"ssCssMatch"' : 'ssCssMatch 1';
            document.getElementsBySelector.matchValue = matchValue;
        }

        // Add the rule that will be used to match elements
        styleSheet.insertRule(cssSelector + ' { counter-increment: ssCssMatch }', 0);

        var docView = document.defaultView;
        for (var i = all.length - 1; i >= 0; i--) {
            var element = all[i];

            // Find elements that contain the special css attribute, i.e.
            // the elements that match the specified selector.
            if (docView.getComputedStyle(element, null).counterIncrement === matchValue) {
                matches[matches.length] = element;
            }
        }

        styleSheet.deleteRule(0);
    }

    if (matches.length > 1) {
        matches.reverse();
    }
    return matches;
}

///////////////////////////////////////////////////////////////////////////////
// Object Extensions

Object.__typeName = 'Object';
Object.__baseType = null;

Object.parse = function Object$parse(s) {
    return eval(s);
}

Object.getKeyCount = function Object$getKeyCount(d) {
    var count = 0;
    for (var n in d) {
        count++;
    }
    return count;
}

Object.clearKeys = function Object$clearKeys(d) {
    for (var n in d) {
        delete d[n];
    }
}

Object.keyExists = function Object$keyExists(d, key) {
    return d[key] !== undefined;
}

///////////////////////////////////////////////////////////////////////////////
// Function Extensions

Function.parse = function Function$parse(s) {
    if (!Function._parseCache) {
        Function._parseCache = { };
    }

    var fn = Function._parseCache[s];
    if (!fn) {
        try {
            eval('fn = ' + s);
            if (typeof(fn) != 'function') {
                fn = null;
            }
            else {
                Function._parseCache[s] = fn;
            }
        }
        catch (ex) {
        }
    }
    return fn;
}

Function.prototype.invoke = function Function$invoke() {
    this.apply(null, arguments);
}

///////////////////////////////////////////////////////////////////////////////
// Boolean Extensions

Boolean.__typeName = 'Boolean';

Boolean.parse = function Boolean$parse(s) {
    return (s.toLowerCase() == 'true');
}

///////////////////////////////////////////////////////////////////////////////
// Number Extensions

Number.__typeName = 'Number';

Number.parse = function Number$parse(s) {
    if (!s || !s.length) {
        return 0;
    }
    if ((s.indexOf('.') >= 0) || (s.indexOf('e') >= 0) ||
        s.endsWith('f') || s.endsWith('F')) {
        return parseFloat(s);
    }
    return parseInt(s);
}

Number.prototype.format = function Number$format(format, useLocale) {
    if (isNullOrUndefined(format) ||
        (format.length == 0) || (format == 'i')) {
        if (useLocale) {
            return this.toLocaleString();
        }
        else {
            return this.toString();
        }
    }

    return this._netFormat(format, useLocale);
}

Number._commaFormat = function Number$_commaFormat(number, groups, decimal, comma) {
    var decimalPart = null;
    var decimalIndex = number.indexOf(decimal);
    if (decimalIndex > 0) {
        decimalPart = number.substr(decimalIndex);
        number = number.substr(0, decimalIndex);
    }

    var negative = number.startsWith('-');
    if (negative) {
        number = number.substr(1);
    }

    var groupIndex = 0;
    var groupSize = groups[groupIndex];
    if (number.length < groupSize) {
        return decimalPart ? number + decimalPart : number;
    }

    var index = number.length;
    var s = '';
    var done = false;
    while (!done) {
        var length = groupSize;
        var startIndex = index - length;
        if (startIndex < 0) {
            groupSize += startIndex;
            length += startIndex;
            startIndex = 0;
            done = true;
        }
        if (!length) {
            break;
        }
        
        var part = number.substr(startIndex, length);
        if (s.length) {
            s = part + comma + s;
        }
        else {
            s = part;
        }
        index -= length;

        if (groupIndex < groups.length - 1) {
            groupIndex++;
            groupSize = groups[groupIndex];
        }
    }

    if (negative) {
        s = '-' + s;
    }    
    return decimalPart ? s + decimalPart : s;
}

Number.prototype._netFormat = function Number$_netFormat(format, useLocale) {
    var nf = useLocale ? CultureInfo.Current.numberFormat : CultureInfo.Neutral.numberFormat;

    var s = '';    
    var precision = -1;
    
    if (format.length > 1) {
        precision = parseInt(format.substr(1));
    }

    var fs = format.charAt(0);
    switch (fs) {
        case 'd': case 'D':
            s = parseInt(Math.abs(this)).toString();
            if (precision != -1) {
                s = s.padLeft(precision, '0');
            }
            if (this < 0) {
                s = '-' + s;
            }
            break;
        case 'x': case 'X':
            s = parseInt(Math.abs(this)).toString(16);
            if (fs == 'X') {
                s = s.toUpperCase();
            }
            if (precision != -1) {
                s = s.padLeft(precision, '0');
            }
            break;
        case 'e': case 'E':
            if (precision == -1) {
                s = this.toExponential();
            }
            else {
                s = this.toExponential(precision);
            }
            if (fs == 'E') {
                s = s.toUpperCase();
            }
            break;
        case 'f': case 'F':
        case 'n': case 'N':
            if (precision == -1) {
                precision = nf.numberDecimalDigits;
            }
            s = this.toFixed(precision).toString();
            if (precision && (nf.numberDecimalSeparator != '.')) {
                var index = s.indexOf('.');
                s = s.substr(0, index) + nf.numberDecimalSeparator + s.substr(index + 1);
            }
            if ((fs == 'n') || (fs == 'N')) {
                s = Number._commaFormat(s, nf.numberGroupSizes, nf.numberDecimalSeparator, nf.numberGroupSeparator);
            }
            break;
        case 'c': case 'C':
            if (precision == -1) {
                precision = nf.currencyDecimalDigits;
            }
            s = Math.abs(this).toFixed(precision).toString();
            if (precision && (nf.currencyDecimalSeparator != '.')) {
                var index = s.indexOf('.');
                s = s.substr(0, index) + nf.currencyDecimalSeparator + s.substr(index + 1);
            }
            s = Number._commaFormat(s, nf.currencyGroupSizes, nf.currencyDecimalSeparator, nf.currencyGroupSeparator);
            if (this < 0) {
                s = String.format(nf.currencyNegativePattern, s);
            }
            else {
                s = String.format(nf.currencyPositivePattern, s);
            }
            break;
        case 'p': case 'P':
            if (precision == -1) {
                precision = nf.percentDecimalDigits;
            }
            s = (Math.abs(this) * 100.0).toFixed(precision).toString();
            if (precision && (nf.percentDecimalSeparator != '.')) {
                var index = s.indexOf('.');
                s = s.substr(0, index) + nf.percentDecimalSeparator + s.substr(index + 1);
            }
            s = Number._commaFormat(s, nf.percentGroupSizes, nf.percentDecimalSeparator, nf.percentGroupSeparator);
            if (this < 0) {
                s = String.format(nf.percentNegativePattern, s);
            }
            else {
                s = String.format(nf.percentPositivePattern, s);
            }
            break;
    }

    return s;
}

///////////////////////////////////////////////////////////////////////////////
// Math Extensions

Math.truncate = function Math$truncate(n) {
    return (n >= 0) ? Math.floor(n) : Math.ceil(n);
}

///////////////////////////////////////////////////////////////////////////////
// String Extensions

String.__typeName = 'String';
String.Empty = '';

String.compare = function String$compare(s1, s2, ignoreCase) {
    if (ignoreCase) {
        if (s1) {
            s1 = s1.toUpperCase();
        }
        if (s2) {
            s2 = s2.toUpperCase();
        }
    }
    s1 = s1 || '';
    s2 = s2 || '';

    if (s1 == s2) {
        return 0;
    }
    if (s1 < s2) {
        return -1;
    }
    return 1;
}

String.prototype.compareTo = function String$compareTo(s, ignoreCase) {
    return String.compare(this, s, ignoreCase);
}

String.prototype.endsWith = function String$endsWith(suffix) {
    if (!suffix.length) {
        return true;
    }
    if (suffix.length > this.length) {
        return false;
    }
    return (this.substr(this.length - suffix.length) == suffix);
}

String.equals = function String$equals1(s1, s2, ignoreCase) {
    return String.compare(s1, s2, ignoreCase) == 0;
}

String._format = function String$_format(format, values, useLocale) {
    if (!String._formatRE) {
        String._formatRE = /(\{[^\}^\{]+\})/g;
    }

    return format.replace(String._formatRE,
                          function(str, m) {
                              var index = parseInt(m.substr(1));
                              var value = values[index + 1];
                              if (isNullOrUndefined(value)) {
                                  return '';
                              }
                              if (value.format) {
                                  var formatSpec = null;
                                  var formatIndex = m.indexOf(':');
                                  if (formatIndex > 0) {
                                      formatSpec = m.substring(formatIndex + 1, m.length - 1);
                                  }
                                  return value.format.call(value, formatSpec, useLocale);
                              }
                              else {
                                  if (useLocale) {
                                      return value.toLocaleString();
                                  }
                                  return value.toString();
                              }
                          });
}

String.format = function String$format(format) {
    return String._format(format, arguments, /* useLocale */ false);
}

String.fromChar = function String$fromChar(ch, count) {
    var s = ch;
    for (var i = 1; i < count; i++) {
        s += ch;
    }
    return s;
}

String.prototype.htmlDecode = function String$htmlDecode() {
    if (!String._htmlDecRE) {
        String._htmlDecMap = { '&amp;': '&', '&lt;': '<', '&gt;': '>', '&quot;': '"' };
        String._htmlDecRE = /(&amp;|&lt;|&gt;|&quot;)/gi;
    }

    var s = this;
    s = s.replace(String._htmlDecRE,
                  function String$htmlDecode$replace(str, m) {
                      return String._htmlDecMap[m];
                  });
    return s;
}

String.prototype.htmlEncode = function String$htmlEncode() {
    if (!String._htmlEncRE) {
        String._htmlEncMap = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' };
        String._htmlEncRE = /([&<>"])/g;
    }

    var s = this;
    if (String._htmlEncRE.test(s)) {
        s = s.replace(String._htmlEncRE,
                      function String$htmlEncode$replace(str, m) {
                          return String._htmlEncMap[m];
                      });
    }
    return s;
}

String.prototype.indexOfAny = function String$indexOfAny(chars, startIndex, count) {
    var length = this.length;
    if (!length) {
        return -1;
    }

    startIndex = startIndex || 0;
    count = count || length;

    var endIndex = startIndex + count - 1;
    if (endIndex >= length) {
        endIndex = length - 1;
    }

    for (var i = startIndex; i <= endIndex; i++) {
        if (chars.indexOf(this.charAt(i)) >= 0) {
            return i;
        }
    }
    return -1;
}

String.prototype.insert = function String$insert(index, value) {
    if (!value) {
        return this;
    }
    if (!index) {
        return value + this;
    }
    var s1 = this.substr(0, index);
    var s2 = this.substr(index);
    return s1 + value + s2;
}

String.isNullOrEmpty = function String$isNullOrEmpty(s) {
    return !s || !s.length;
}

String.prototype.lastIndexOfAny = function String$lastIndexOfAny(chars, startIndex, count) {
    var length = this.length;
    if (!length) {
        return -1;
    }

    startIndex = startIndex || length - 1;
    count = count || length;

    var endIndex = startIndex - count + 1;
    if (endIndex < 0) {
        endIndex = 0;
    }

    for (var i = startIndex; i >= endIndex; i--) {
        if (chars.indexOf(this.charAt(i)) >= 0) {
            return i;
        }
    }
    return -1;
}

String.localeFormat = function String$localeFormat(format) {
    return String._format(format, arguments, /* useLocale */ true);
}

String.prototype.padLeft = function String$padLeft(totalWidth, ch) {
    if (this.length < totalWidth) {
        ch = ch || ' ';
        return String.fromChar(ch, totalWidth - this.length) + this;
    }
    return this;
}

String.prototype.padRight = function String$padRight(totalWidth, ch) {
    if (this.length < totalWidth) {
        ch = ch || ' ';
        return this + String.fromChar(ch, totalWidth - this.length);
    }
    return this;
}

String.prototype.quote = function String$quote() {
    if (!String._quoteMap) {
        String._quoteMap = { '\\' : '\\\\',
                             '\'' : '\\\'', '"' : '\\"',
                             '\r' : '\\r', '\n' : '\\n', '\t' : '\\t', '\f' : '\\f',
                             '\b' : '\\b' };
    }
    if (!String._quoteRE || !RegExp._cacheable) {
        String._quoteRE = new RegExp("([\'\"\\\\\x00-\x1F\x7F-\uFFFF])", "g");
    }

    var s = this;
    if (String._quoteSkipTest || String._quoteRE.test(s)) {
        s = this.replace(String._quoteRE,
                         function String$quote$replace(str, m) {
                             var c = String._quoteMap[m];
                             if (c) {
                                 return c;
                             }
                             c = m.charCodeAt(0);
                             return '\\u' + c.toString(16).toUpperCase().padLeft(4, '0');
                         });
    }
    return '"' + s + '"';
}

String.prototype.remove = function String$remove(index, count) {
    if (!count || ((index + count) > this.length)) {
        return this.substr(0, index);
    }
    return this.substr(0, index) + this.substr(index + count);
}

String.prototype._replace = String.prototype.replace;
String.prototype.replace = function String$replace(oldValue, newValue) {
    if (oldValue.constructor == String) {
        newValue = newValue || '';
        return this.split(oldValue).join(newValue);
    }
    return String.prototype._replace.call(this, oldValue, newValue);
}

String.prototype.startsWith = function String$startsWith(prefix) {
    if (!prefix.length) {
        return true;
    }
    if (prefix.length > this.length) {
        return false;
    }
    return (this.substr(0, prefix.length) == prefix);
}

String.prototype.trim = function String$trim() {
    return this.trimEnd().trimStart();
}

String.prototype.trimEnd = function String$trimEnd() {
    return this.replace(/\s*$/, '');
}

String.prototype.trimStart = function String$trimStart() {
    return this.replace(/^\s*/, '');
}

String.prototype.unquote = function String$unquote() {
    return eval('(' + this + ')');
}

///////////////////////////////////////////////////////////////////////////////
// Array Extensions

Array.__typeName = 'Array';

Array.prototype.add = function Array$add(item) {
    this[this.length] = item;
}

Array.prototype.addRange = function Array$addRange(items) {
    if (!items) {
        return;
    }
    var length = items.length;
    for (var index = 0; index < length; index++) {
        this[this.length] = items[index];
    }
}

Array.prototype.aggregate = function Array$aggregate(seed, callback) {
    var length = this.length;
    for (var index = 0; index < length; index++) {
        seed = callback(seed, this[index], index, this);
    }
    return seed;
}

Array.prototype.clear = function Array$clear() {
    if (this.length > 0) {
        this.splice(0, this.length);
    }
}

Array.prototype.clone = function Array$clone() {
    var length = this.length;
    var array = new Array(length);
    for (var index = 0; index < length; index++) {
        array[index] = this[index];
    }
    return array;
}

Array.prototype.contains = function Array$contains(item) {
    var index = this.indexOf(item);
    return (index >= 0);
}

Array.prototype.dequeue = function Array$dequeue() {
    return this.shift();
}

Array.prototype.enqueue = function Array$enqueue(item) {
    // We record that this array instance is a queue, so we
    // can implement the right behavior in the peek method.
    this._queue = true;
    this.push(item);
}

Array.prototype.peek = function Array$peek() {
    if (this.length) {
        var index = this._queue ? 0 : this.length - 1;
        return this[index];
    }
    return null;
}

if (!Array.prototype.every) {
    Array.prototype.every = function Array$every(callback) {
        for (var i = this.length - 1; i >= 0; i--) {
            if (!callback(this[i], i, this)) {
                return false;
            }
        }
        return true;
    }
}

Array.prototype.extract = function Array$extract(index, count) {
    if (!count) {
        return this.slice(index);
    }
    return this.slice(index, index + count);
}

if (!Array.prototype.filter) {
    Array.prototype.filter = function Array$filter(callback) {
        var filtered = [];
        for (var i = 0; i < this.length; i++) {
            if (callback(this[i], i, this)) {
                filtered.add(this[i]);
            }
        }
        return filtered;
    }
}

if (!Array.prototype.forEach) {
    Array.prototype.forEach = function Array$forEach(callback) {
        for (var i = 0; i < this.length; i++) {
            callback(this[i], i, this);
        }
    }
}

Array.prototype.groupBy = function Array$groupBy(callback) {
    var length = this.length;
    var groups = [];
    var keys = { };
    for (var index = 0; index < length; index++) {
        var key = callback(this[index], index);
        if (String.isNullOrEmpty(key)) {
            continue;
        }
        var items = keys[key];
        if (!items) {
            items = [];
            items.key = key;

            keys[key] = items;
            groups.add(items);
        }
        items.add(this[index]);
    }
    return groups;
}

Array.prototype.index = function Array$index(callback) {
    var length = this.length;
    var items = { };
    for (var index = 0; index < length; index++) {
        var key = callback(this[index], index);
        if (String.isNullOrEmpty(key)) {
            continue;
        }
        items[key] = this[index];
    }
    return items;
}

Array.prototype.indexOf = function Array$indexOf(item) {
    var length = this.length;
    if (length) {
        for (var index = 0; index < length; index++) {
            if (this[index] === item) {
                return index;
            }
        }
    }
    return -1;
}

Array.prototype.insert = function Array$insert(index, item) {
    this.splice(index, 0, item);
}

Array.prototype.insertRange = function Array$insertRange(index, items) {
    this.splice(index, 0, items);
}

if (!Array.prototype.map) {
    Array.prototype.map = function Array$map(callback) {
        var mapped = new Array(this.length);
        for (var i = this.length - 1; i >= 0; i--) {
            mapped[i] = callback(this[i], i, this);
        }
        return mapped;
    }
}

Array.parse = function Array$parse(s) {
    return eval('(' + s + ')');
}

Array.prototype.remove = function Array$remove(item) {
    var index = this.indexOf(item);
    if (index >= 0) {
        this.splice(index, 1);
        return true;
    }
    return false;
}

Array.prototype.removeAt = function Array$removeAt(index) {
    return this.splice(index, 1)[0];
}

Array.prototype.removeRange = function Array$removeRange(index, count) {
    return this.splice(index, count);
}

if (!Array.prototype.some) {
    Array.prototype.some = function Array$some(callback) {
        for (var i = this.length - 1; i >= 0; i--) {
            if (callback(this[i], i, this)) {
                return true;
            }
        }
        return false;
    }
}

///////////////////////////////////////////////////////////////////////////////
// RegExp Extensions

RegExp.__typeName = 'RegExp';

RegExp.parse = function RegExp$parse(s) {
    if (s.startsWith('/')) {
        var endSlashIndex = s.lastIndexOf('/');
        if (endSlashIndex > 1) {
            var expression = s.substring(1, endSlashIndex);
            var flags = s.substr(endSlashIndex + 1);
            return new RegExp(expression, flags);
        }
    }

    return null;    
}

///////////////////////////////////////////////////////////////////////////////
// Date Extensions

Date.__typeName = 'Date';

Date.get_now = function Date$get_now() {
    return new Date();
}

Date.get_today = function Date$get_today() {
    var d = new Date();
    return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

Date.prototype.format = function Date$format(format, useLocale) {
    if (isNullOrUndefined(format) ||
        (format.length == 0) || (format == 'i')) {
        if (useLocale) {
            return this.toLocaleString();
        }
        else {
            return this.toString();
        }
    }
    if (format == 'id') {
        if (useLocale) {
            return this.toLocaleDateString();
        }
        else {
            return this.toDateString();
        }
    }
    if (format == 'it') {
        if (useLocale) {
            return this.toLocaleTimeString();
        }
        else {
            return this.toTimeString();
        }
    }

    return this._netFormat(format, useLocale);
}

Date.prototype._netFormat = function Date$_netFormat(format, useLocale) {
    var dtf = useLocale ? CultureInfo.Current.dateFormat : CultureInfo.Neutral.dateFormat;
    var useUTC = false;

    if (format.length == 1) {
        switch (format) {
            case 'f': format = dtf.longDatePattern + ' ' + dtf.shortTimePattern;
            case 'F': format = dtf.dateTimePattern; break;

            case 'd': format = dtf.shortDatePattern; break;
            case 'D': format = dtf.longDatePattern; break;

            case 't': format = dtf.shortTimePattern; break;
            case 'T': format = dtf.longTimePattern; break;

            case 'g': format = dtf.shortDatePattern + ' ' + dtf.shortTimePattern; break;
            case 'G': format = dtf.shortDatePattern + ' ' + dtf.longTimePattern; break;

            case 'R': case 'r': format = dtf.gmtDateTimePattern; useUTC = true; break;
            case 'u': format = dtf.universalDateTimePattern; useUTC = true; break;
            case 'U': format = dtf.dateTimePattern; useUTC = true; break;

            case 's': format = dtf.sortableDateTimePattern; break;
        }
    }

    if (format.charAt(0) == '%') {
        format = format.substr(1);
    }

    if (!Date._formatRE) {
        Date._formatRE = /dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|y|hh|h|HH|H|mm|m|ss|s|tt|t|fff|ff|f|zzz|zz|z/g;
    }

    var re = Date._formatRE;    
    var sb = new StringBuilder();
    var dt = this;
    if (useUTC) {
        dt = new Date(Date.UTC(dt.getUTCFullYear(), dt.getUTCMonth(), dt.getUTCDate(),
                               dt.getUTCHours(), dt.getUTCMinutes(), dt.getUTCSeconds(), dt.getUTCMilliseconds()));
    }

    re.lastIndex = 0;
    while (true) {
        var index = re.lastIndex;
        var match = re.exec(format);

        sb.append(format.slice(index, match ? match.index : format.length));
        if (!match) {
            break;
        }

        var fs = match[0];
        var part = fs;
        switch (fs) {
            case 'dddd':
                part = dtf.dayNames[dt.getDay()];
                break;
            case 'ddd':
                part = dtf.shortDayNames[dt.getDay()];
                break;
            case 'dd':
                part = dt.getDate().toString().padLeft(2, '0');
                break;
            case 'd':
                part = dt.getDate();
                break;
            case 'MMMM':
                part = dtf.monthNames[dt.getMonth()];
                break;
            case 'MMM':
                part = dtf.shortMonthNames[dt.getMonth()];
                break;
            case 'MM':
                part = (dt.getMonth() + 1).toString().padLeft(2, '0');
                break;
            case 'M':
                part = (dt.getMonth() + 1);
                break;
            case 'yyyy':
                part = dt.getFullYear();
                break;
            case 'yy':
                part = (dt.getFullYear() % 100).toString().padLeft(2, '0');
                break;
            case 'y':
                part = (dt.getFullYear() % 100);
                break;
            case 'h': case 'hh':
                part = dt.getHours() % 12;
                if (!part) {
                    part = '12';
                }
                else if (fs == 'hh') {
                    part = part.toString().padLeft(2, '0');
                }
                break;
            case 'HH':
                part = dt.getHours().toString().padLeft(2, '0');
                break;
            case 'H':
                part = dt.getHours();
                break;
            case 'mm':
                part = dt.getMinutes().toString().padLeft(2, '0');
                break;
            case 'm':
                part = dt.getMinutes();
                break;
            case 'ss':
                part = dt.getSeconds().toString().padLeft(2, '0');
                break;
            case 's':
                part = dt.getSeconds();
                break;
            case 't': case 'tt':
                part = (dt.getHours() < 12) ? dtf.amDesignator : dtf.pmDesignator;
                if (fs == 't') {
                    part = part.charAt(0);
                }
                break;
            case 'fff':
                part = dt.getMilliseconds().toString().padLeft(3, '0');
                break;
            case 'ff':
                part = dt.getMilliseconds().toString().padLeft(3).substr(0, 2);
                break;
            case 'f':
                part = dt.getMilliseconds().toString().padLeft(3).charAt(0);
                break;
            case 'z':
                part = dt.getTimezoneOffset() / 60;
                part = ((part >= 0) ? '-' : '+') + Math.floor(Math.abs(part));
                break;
            case 'zz': case 'zzz':
                part = dt.getTimezoneOffset() / 60;
                part = ((part >= 0) ? '-' : '+') + Math.floor(Math.abs(part)).toString().padLeft(2, '0');
                if (fs == 'zzz') {
                    part += dtf.timeSeparator + Math.abs(dt.getTimezoneOffset() % 60).toString().padLeft(2, '0');
                }
                break;
        }
        sb.append(part);
    }

    return sb.toString();
}

Date._parse = Date.parse;
Date.parse = function Date$parse(s) {
    // Date.parse by default returns the number of milliseconds
    // and not an actual Date instance
    return new Date(Date._parse(s));
}

///////////////////////////////////////////////////////////////////////////////
// Error Extensions

Error.__typeName = 'Error';

Error.create = function Error$create(message, userData, innerException) {
    var e = new Error(message);
    if (userData) {
        e.userData = userData;
    }
    if (innerException) {
        e.innerException = innerException;
    }
    return e;
}

///////////////////////////////////////////////////////////////////////////////
// Debug Extensions

if (!Debug._fail) {
    Debug._fail = function Debug$_fail(message) {
        Debug.writeln(message);
        eval('debugger;');
    }
}

Debug.assert = function Debug$assert(condition, message) {
    if (!condition) {
        message = 'Assert failed: ' + message;
        if (confirm(message + '\r\n\r\nBreak into debugger?')) {
            Debug._fail(message);
        }
    }
}

Debug._dumpCore = function Debug$_dumpCore(sb, object, name, indentation, dumpedObjects) {
    if (object === null) {
        sb.appendLine(indentation + name + ': null');
        return;
    }
    switch (typeof(object)) {
        case 'undefined':
            sb.appendLine(indentation + name + ': undefined');
            break;
        case 'number':
        case 'string':
        case 'boolean':
            sb.appendLine(indentation + name + ': ' + object);
            break;
        default:
            if (Date.isInstance(object) || RegExp.isInstance(object)) {
                sb.appendLine(indentation + name + ': ' + object);
                break;
            }

            if (dumpedObjects.contains(object)) {
                sb.appendLine(indentation + name + ': ...');
                break;
            }
            dumpedObjects.add(object);

            var type = Type.getInstanceType(object);
            var typeName = type.get_fullName();
            var recursiveIndentation = indentation + '  ';

            if (IArray.isInstance(object)) {
                sb.appendLine(indentation + name + ': {' + typeName + '}');
                var length = object.getLength();
                for (var i = 0; i < length; i++) {
                    Debug._dumpCore(sb, object.getItem(i), '[' + i + ']', recursiveIndentation, dumpedObjects);
                }
            }
            else {
                if (object.tagName) {
                    sb.appendLine(indentation + name + ': <' + object.tagName + '>');
                    var attributes = object.attributes;
                    for (var i = 0; i < attributes.length; i++) {
                        var attrValue = attributes[i].nodeValue;
                        if (attrValue) {
                            Debug._dumpCore(sb, attrValue, attributes[i].nodeName, recursiveIndentation, dumpedObjects);
                        }
                    }
                }
                else {
                    sb.appendLine(indentation + name + ': {' + typeName + '}');
                    for (var field in object) {
                        var v = object[field];
                        if (!Function.isInstance(v)) {
                            Debug._dumpCore(sb, v, field, recursiveIndentation, dumpedObjects);
                        }
                    }
                }
            }

            dumpedObjects.remove(object);
            break;
    }
}

Debug.dump = function Debug$dump(object, name) {
    if ((!name || !name.length) && (object !== null)) {
        name = Type.getInstanceType(object).get_fullName();
    }
    if (!name || !name.length) {
        return;
    }
    var sb = new StringBuilder();
    Debug._dumpCore(sb, object, name, '', []);
    Debug.writeLine(sb.toString());
}

Debug.fail = function Debug$fail(message) {
    Debug._fail(message);
}

Debug.inspect = function Debug$inspect(object, name) {
    var dumped = false;
    if (window.debugService) {
        dumped = window.debugService.inspect(name, object);
    }
    if (!dumped) {
        Debug.dump(object, name);
    }
}

Debug.writeLine = function Debug$writeLine(message) {
    if (window.debugService) {
        window.debugService.trace(message);
        return;
    }
    Debug.writeln(message);

    var traceTextBox = $('_traceTextBox');
    if (traceTextBox) {
        traceTextBox.value = traceTextBox.value + '\r\n' + message;
    }
}

Debug.__typeName = 'Debug';

///////////////////////////////////////////////////////////////////////////////
// Type System Implementation

var Type = Function;
Type.__typeName = 'Type';

var __Namespace = function(name) {
    this.__typeName = name;
}
__Namespace.prototype = {
    __namespace: true,
    getName: function() {
        return this.__typeName;
    }
}

Type.createNamespace = function Type$createNamespace(name) {
    if (!window.__namespaces) {
        window.__namespaces = {};
    }
    if (!window.__rootNamespaces) {
        window.__rootNamespaces = [];
    }

    if (window.__namespaces[name]) {
        return;
    }

    var ns = window;
    var nameParts = name.split('.');

    for (var i = 0; i < nameParts.length; i++) {
        var part = nameParts[i];
        var nso = ns[part];
        if (!nso) {
            ns[part] = nso = new __Namespace(nameParts.slice(0, i + 1).join('.'));
            if (i == 0) {
                window.__rootNamespaces.add(nso);
            }
        }
        ns = nso;
    }

    window.__namespaces[name] = ns;
}

Type.prototype.createClass = function Type$createClass(name, baseType, interfaceType) {
    this.prototype.constructor = this;
    this.__typeName = name;
    this.__class = true;
    this.__baseType = baseType || Object;
    if (baseType) {
        this.__basePrototypePending = true;
    }

    if (interfaceType) {
        this.__interfaces = [];
        for (var i = 2; i < arguments.length; i++) {
            interfaceType = arguments[i];
            this.__interfaces.add(interfaceType);
        }
    }
}

Type.prototype.createInterface = function Type$createInterface(name) {
    this.__typeName = name;
    this.__interface = true;
}

Type.prototype.createEnum = function Type$createEnum(name, flags) {
    for (var field in this.prototype) {
         this[field] = this.prototype[field];
    }

    this.__typeName = name;
    this.__enum = true;
    if (flags) {
        this.__flags = true;
    }
}

Type.prototype.setupBase = function Type$setupBase() {
    if (this.__basePrototypePending) {
        var baseType = this.__baseType;
        if (baseType.__basePrototypePending) {
            baseType.setupBase();
        }

        for (var memberName in baseType.prototype) {
            var memberValue = baseType.prototype[memberName];
            if (!this.prototype[memberName]) {
                this.prototype[memberName] = memberValue;
            }
        }

        delete this.__basePrototypePending;
    }
}

if (!Type.prototype.resolveInheritance) {
    // This function is not used by Script#; Visual Studio relies on it
    // for JavaScript IntelliSense support of derived types.
    Type.prototype.resolveInheritance = Type.prototype.setupBase;
}

Type.prototype.constructBase = function Type$constructBase(instance, args) {
    if (this.__basePrototypePending) {
        this.setupBase();
    }

    if (!args) {
        this.__baseType.apply(instance);
    }
    else {
        this.__baseType.apply(instance, args);
    }
}

Type.prototype.callBase = function Type$callBase(instance, name, args) {
    var baseMethod = this.__baseType.prototype[name];
    if (!args) {
        return baseMethod.apply(instance);
    }
    else {
        return baseMethod.apply(instance, args);
    }
}

Type.prototype.get_baseType = function Type$get_baseType() {
    return this.__baseType || null;
}

Type.prototype.get_fullName = function Type$get_fullName() {
    return this.__typeName;
}

Type.prototype.get_name = function Type$get_name() {
    var fullName = this.__typeName;
    var nsIndex = fullName.lastIndexOf('.');
    if (nsIndex > 0) {
        return fullName.substr(nsIndex + 1);
    }
    return fullName;
}

Type.prototype.isInstance = function Type$isInstance(instance) {
    if (isNullOrUndefined(instance)) {
        return false;
    }
    if ((this == Object) || (instance instanceof this)) {
        return true;
    }

    var type = Type.getInstanceType(instance);
    return this.isAssignableFrom(type);
}

Type.prototype.isAssignableFrom = function Type$isAssignableFrom(type) {
    if ((this == Object) || (this == type)) {
        return true;
    }
    if (this.__class) {
        var baseType = type.__baseType;
        while (baseType) {
            if (this == baseType) {
                return true;
            }
            baseType = baseType.__baseType;
        }
    }
    else if (this.__interface) {
        var interfaces = type.__interfaces;
        if (interfaces && interfaces.contains(this)) {
            return true;
        }

        var baseType = type.__baseType;
        while (baseType) {
            interfaces = baseType.__interfaces;
            if (interfaces && interfaces.contains(this)) {
                return true;
            }
            baseType = baseType.__baseType;
        }
    }
    return false;
}

Type.isClass = function Type$isClass(type) {
    return (type.__class == true);
}

Type.isEnum = function Type$isEnum(type) {
    return (type.__enum == true);
}

Type.isFlagsEnum = function Type$isFlagsEnum(type) {
    return ((type.__enum == true) && (type.__flags == true));
}

Type.isInterface = function Type$isInterface(type) {
    return (type.__interface == true);
}

Type.canCast = function Type$canCast(instance, type) {
    return type.isInstance(instance);
}

Type.safeCast = function Type$safeCast(instance, type) {
    if (type.isInstance(instance)) {
        return instance;
    }
    return null;
}

Type.getInstanceType = function Type$getInstanceType(instance) {
    var ctor = null;

    // NOTE: We have to catch exceptions because the constructor
    //       cannot be looked up on native COM objects
    try {
        ctor = instance.constructor;
    }
    catch (ex) {
    }
    if (!ctor || !ctor.__typeName) {
        ctor = Object;
    }
    return ctor;
}

Type.getType = function Type$getType(typeName) {
    if (!typeName) {
        return null;
    }

    if (!Type.__typeCache) {
        Type.__typeCache = {};
    }

    var type = Type.__typeCache[typeName];
    if (!type) {
        type = eval(typeName);
        Type.__typeCache[typeName] = type;
    }
    return type;
}

Type.parse = function Type$parse(typeName) {
    return Type.getType(typeName);
}

///////////////////////////////////////////////////////////////////////////////
// Enum

var Enum = function() {
}
Enum.createClass('Enum');

Enum.parse = function Enum$parse(enumType, s) {
    var values = enumType.prototype;
    if (!enumType.__flags) {
        for (var f in values) {
            if (f === s) {
                return values[f];
            }
        }
    }
    else {
        var parts = s.split('|');
        var value = 0;
        var parsed = true;

        for (var i = parts.length - 1; i >= 0; i--) {
            var part = parts[i].trim();
            var found = false;

            for (var f in values) {
                if (f === part) {
                    value |= values[f];
                    found = true;
                    break;
                }
            }
            if (!found) {
                parsed = false;
                break;
            }
        }

        if (parsed) {
            return value;
        }
    }
    throw 'Invalid Enumeration Value';
}

Enum.toString = function Enum$toString(enumType, value) {
    var values = enumType.prototype;
    if (!enumType.__flags || (value === 0)) {
        for (var i in values) {
            if (values[i] === value) {
                return i;
            }
        }
        throw 'Invalid Enumeration Value';
    }
    else {
        var parts = [];
        for (var i in values) {
            if (values[i] & value) {
                if (parts.length) {
                    parts.add(' | ');
                }
                parts.add(i);
            }
        }
        if (!parts.length) {
            throw 'Invalid Enumeration Value';
        }
        return parts.join('');
    }
}

///////////////////////////////////////////////////////////////////////////////
// Delegate

var Delegate = function() {
}
Delegate.createClass('Delegate');

Delegate.Null = function() { }

Delegate._create = function Delegate$_create(targets) {
    var delegate = function() {
        if (targets.length == 2) {
            return targets[1].apply(targets[0], arguments);
        }
        else {
            for (var i = 0; i < targets.length; i += 2) {
                targets[i + 1].apply(targets[i], arguments);
            }
            return null;
        }
    };
    delegate.invoke = delegate;
    delegate._targets = targets;

    return delegate;
}

Delegate.create = function Delegate$create(object, method) {
    if (!object) {
        method.invoke = method;
        return method;
    }
    return Delegate._create([object, method]);
}

Delegate.combine = function Delegate$combine(delegate1, delegate2) {
    if (!delegate1) {
        if (!delegate2._targets) {
            return Delegate.create(null, delegate2);
        }
        return delegate2;
    }
    if (!delegate2) {
        if (!delegate1._targets) {
            return Delegate.create(null, delegate1);
        }
        return delegate1;
    }

    var targets1 = delegate1._targets ? delegate1._targets : [null, delegate1];
    var targets2 = delegate2._targets ? delegate2._targets : [null, delegate2];

    return Delegate._create(targets1.concat(targets2));
}

Delegate.remove = function Delegate$remove(delegate1, delegate2) {
    if (!delegate1 || (delegate1 === delegate2)) {
        return null;
    }
    if (!delegate2) {
        return delegate1;
    }

    var targets = delegate1._targets;
    var object = null;
    var method;
    if (delegate2._targets) {
        object = delegate2._targets[0];
        method = delegate2._targets[1];
    }
    else {
        method = delegate2;
    }

    for (var i = 0; i < targets.length; i += 2) {
        if ((targets[i] === object) && (targets[i + 1] === method)) {
            if (targets.length == 2) {
                return null;
            }
            targets.splice(i, 2);
            return Delegate._create(targets);
        }
    }

    return delegate1;
}


Delegate.createExport = function Delegate$createExport(delegate, multiUse) {
    var name = '__' + (new Date()).valueOf();
    Delegate[name] = function() {
        if (!multiUse) {
            Delegate.deleteExport(name);
        }
        delegate.apply(null, arguments);
    };

    return name;
}

Delegate.deleteExport = function Delegate$deleteExport(name) {
    if (Delegate[name]) {
        delete Delegate[name];
    }
}

Delegate.clearExport = function Delegate$clearExport(name) {
    if (Delegate[name]) {
        Delegate[name] = Delegate.Null;
    }
}

///////////////////////////////////////////////////////////////////////////////
// CultureInfo

var CultureInfo = function(name, numberFormat, dateFormat) {
    this.name = name;
    this.numberFormat = numberFormat;
    this.dateFormat = dateFormat;
}
CultureInfo.createClass('CultureInfo');

CultureInfo.Neutral = new CultureInfo('en-US',
    {
        naNSymbol: 'NaN',
        negativeSign: '-',
        positiveSign: '+',
        negativeInfinityText: '-Infinity',
        positiveInfinityText: 'Infinity',
        
        percentSymbol: '%',
        percentGroupSizes: [3],
        percentDecimalDigits: 2,
        percentDecimalSeparator: '.',
        percentGroupSeparator: ',',
        percentPositivePattern: '{0} %',
        percentNegativePattern: '-{0} %',

        currencySymbol:'$',
        currencyGroupSizes: [3],
        currencyDecimalDigits: 2,
        currencyDecimalSeparator: '.',
        currencyGroupSeparator: ',',
        currencyNegativePattern: '(${0})',
        currencyPositivePattern: '${0}',

        numberGroupSizes: [3],
        numberDecimalDigits: 2,
        numberDecimalSeparator: '.',
        numberGroupSeparator: ','
    },
    {
        amDesignator: 'AM',
        pmDesignator: 'PM',

        dateSeparator: '/',
        timeSeparator: ':',

        gmtDateTimePattern: 'ddd, dd MMM yyyy HH:mm:ss \'GMT\'',
        universalDateTimePattern: 'yyyy-MM-dd HH:mm:ssZ',
        sortableDateTimePattern: 'yyyy-MM-ddTHH:mm:ss',
        dateTimePattern: 'dddd, MMMM dd, yyyy h:mm:ss tt',

        longDatePattern: 'dddd, MMMM dd, yyyy',
        shortDatePattern: 'M/d/yyyy',

        longTimePattern: 'h:mm:ss tt',
        shortTimePattern: 'h:mm tt',

        firstDayOfWeek: 0,
        dayNames: ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'],
        shortDayNames: ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'],
        minimizedDayNames: ['Su','Mo','Tu','We','Th','Fr','Sa'],

        monthNames: ['January','February','March','April','May','June','July','August','September','October','November','December',''],
        shortMonthNames: ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec','']
    });
CultureInfo.Current = CultureInfo.Neutral;

///////////////////////////////////////////////////////////////////////////////
// IArray

var IArray = function() { };
IArray.prototype = {
    getLength: null,
    getItem: null
}
IArray.createInterface('IArray');

///////////////////////////////////////////////////////////////////////////////
// IEnumerator

var IEnumerator = function() { };
IEnumerator.prototype = {
    get_current: null,
    moveNext: null,
    reset: null
}
IEnumerator.createInterface('IEnumerator');

///////////////////////////////////////////////////////////////////////////////
// IEnumerable

var IEnumerable = function() { };
IEnumerable.prototype = {
    getEnumerator: null
}
IEnumerable.createInterface('IEnumerable');

///////////////////////////////////////////////////////////////////////////////
// ArrayEnumerator

var ArrayEnumerator = function ArrayEnumerator$(array) {
    this._array = array;
    this._index = -1;
}
ArrayEnumerator.prototype = {
    get_current: function ArrayEnumerator$get_current() {
        return this._array[this._index];
    },
    moveNext: function ArrayEnumerator$moveNext() {
        this._index++;
        return (this._index < this._array.length);
    },
    reset: function ArrayEnumerator$reset() {
        this._index = -1;
    }
}

ArrayEnumerator.createClass('ArrayEnumerator', null, IEnumerator);

///////////////////////////////////////////////////////////////////////////////
// Array Extensions

Array.__interfaces = [ IArray, IEnumerable ];

Array.prototype.getLength = function Array$getLength() {
    return this.length;
}
Array.prototype.getItem = function Array$getItem(index) {
    return this[index];
}

Array.prototype.getEnumerator = function Array$getEnumerator() {
    return new ArrayEnumerator(this);
}

///////////////////////////////////////////////////////////////////////////////
// IDisposable

var IDisposable = function() { };
IDisposable.prototype = {
    dispose: null
}
IDisposable.createInterface('IDisposable');

///////////////////////////////////////////////////////////////////////////////
// IServiceProvider

var IServiceProvider = function() { };
IServiceProvider.prototype = {
    getService: null
}
IServiceProvider.createInterface('IServiceProvider');

///////////////////////////////////////////////////////////////////////////////
// IServiceContainer

var IServiceContainer = function() { };
IServiceContainer.prototype = {
    registerService: null,
    unregisterService: null
}
IServiceContainer.createInterface('IServiceContainer');

///////////////////////////////////////////////////////////////////////////////
// StringBuilder

var StringBuilder = function StringBuilder$(s) {

    if ((s !== undefined) && (s !== null)) {
        this._parts = [ s ];
    }
    else {
        this._parts = [];
    }
}
StringBuilder.prototype = {
    get_isEmpty: function StringBuilder$get_isEmpty() {
        return (this._parts.length == 0);
    },

    append: function StringBuilder$append(s) {
        if ((s !== undefined) && (s !== null)) {
            this._parts.add(s);
        }
    },

    appendLine: function StringBuilder$appendLine(s) {
        this.append(s);
        this.append('\r\n');
    },

    clear: function StringBuilder$clear() {
        this._parts.clear();
    },

    toString: function StringBuilder$toString() {
        return this._parts.join('');
    }
};

StringBuilder.createClass('StringBuilder');

///////////////////////////////////////////////////////////////////////////////
// EventArgs

var EventArgs = function EventArgs$() {
}
EventArgs.createClass('EventArgs');

EventArgs.Empty = new EventArgs();

///////////////////////////////////////////////////////////////////////////////
// XMLHttpRequest

if (!window.XMLHttpRequest) {
    window.XMLHttpRequest = function() {
        var progIDs = [ 'Msxml2.XMLHTTP', 'Microsoft.XMLHTTP' ];

        for (var i = 0; i < progIDs.length; i++) {
            try {
                var xmlHttp = new ActiveXObject(progIDs[i]);
                return xmlHttp;
            }
            catch (ex) {
            }
        }

        return null;
    }
}

///////////////////////////////////////////////////////////////////////////////
// XMLDocumentParser

var XMLDocumentParser = function() {
}
XMLDocumentParser.createClass('XMLDocumentParser');

XMLDocumentParser.parse = function XMLDocumentParser$parse(markup) {
    if (!window.DOMParser) {
        var progIDs = [ 'Msxml2.DOMDocument.3.0', 'Msxml2.DOMDocument' ];
        
        for (var i = 0; i < progIDs.length; i++) {
            try {
                var xmlDOM = new ActiveXObject(progIDs[i]);
                xmlDOM.async = false;
                xmlDOM.loadXML(markup);
                xmlDOM.setProperty('SelectionLanguage', 'XPath');
                
                return xmlDOM;
            }
            catch (ex) {
            }
        }
    }
    else {
        try {
            var domParser = new DOMParser();
            return domParser.parseFromString(markup, 'text/xml');
        }
        catch (ex) {
        }
    }

    return null;
}

////////////////////////////////////////////////////////////////////////////////
// ScriptLoader

var ScriptLoader = function ScriptLoader(scriptURLs) {
    Debug.assert((scriptURLs) && (scriptURLs.length));
    this._scriptURLs = scriptURLs;
    this._scriptLoadIndex = -1;
}
ScriptLoader.prototype = {
    _scriptURLs: null,
    _loadedHandler: null,
    _errorHandler: null,
    _isIE: false,
    _onLoadHandler: null,
    _onErrorHandler: null,
    _scriptLoadIndex: 0,
    _scriptElements: null,
    _loadedScripts: 0,
    _inError: false,
    _loaded: false,
    
    dispose: function ScriptLoader$dispose() {
        if (this._scriptElements) {
            for (var i = 0; i < this._scriptElements.length; i++) {
                var scriptElement = this._scriptElements[i];
                if (this._isIE) {
                    scriptElement.detachEvent('onreadystatechange', this._onLoadHandler);
                }
                else {
                    scriptElement.detachEvent('onload', this._onLoadHandler);
                    scriptElement.detachEvent('onerror', this._onErrorHandler);
                }
            }
            this._scriptElements = null;
        }
    },
    
    load: function ScriptLoader$load(loadInParallel, timeout, loadedHandler, errorHandler) {
        Debug.assert(loadedHandler);
        Debug.assert(errorHandler);
        this._loadedHandler = loadedHandler;
        this._errorHandler = errorHandler;
        this._isIE = (window.navigator.userAgent.indexOf('MSIE') >= 0);
        this._onLoadHandler = Delegate.create(this, this._onScriptLoad);
        if (!this._isIE) {
            this._onErrorHandler = Delegate.create(this, this._onScriptError);
        }
        this._scriptElements = [];
        if (loadInParallel) {
            for (var i = 0; i < this._scriptURLs.length; i++) {
                this._loadScript(this._scriptURLs[i]);
            }
        }
        else {
            this._scriptLoadIndex++;
            this._loadScript(this._scriptURLs[this._scriptLoadIndex]);
        }
        if (timeout) {
            window.setTimeout(Delegate.create(this, this._onScriptError), timeout);
        }
    },
    
    _loadScript: function ScriptLoader$_loadScript(scriptURL) {
        var scriptElement = document.createElement('SCRIPT');
        if (this._isIE) {
            scriptElement.attachEvent('onreadystatechange', this._onLoadHandler);
        }
        else {
            scriptElement.readyState = 'complete';
            scriptElement.attachEvent('onload', this._onLoadHandler);
            scriptElement.attachEvent('onerror', this._onErrorHandler);
        }
        scriptElement.type = 'text/javascript';
        scriptElement.src = scriptURL;
        this._scriptElements.add(scriptElement);
        document.getElementsByTagName('HEAD')[0].appendChild(scriptElement);
    },
    
    _onScriptError: function ScriptLoader$_onScriptError() {
        if ((!this._inError) && (!this._loaded)) {
            this._inError = true;
            this._errorHandler.invoke(this, EventArgs.Empty);
        }
    },
    
    _onScriptLoad: function ScriptLoader$_onScriptLoad() {
        if (this._inError) {
            return;
        }
        var scriptElement = window.event.srcElement;
        if (!scriptElement.readyState) {
            scriptElement = window.event.currentTarget;
        }

        if ((scriptElement.readyState != 'complete') && (scriptElement.readyState != 'loaded')) {
            return;
        }
        if (this._scriptLoadIndex != -1) {
            this._scriptLoadIndex++;
            if (this._scriptLoadIndex != this._scriptURLs.length) {
                this._loadScript(this._scriptURLs[this._scriptLoadIndex]);
                return;
            }
        }
        else {
            this._loadedScripts++;
            if (this._loadedScripts != this._scriptURLs.length) {
                return;
            }
        }
        this._loaded = true;
        this._loadedHandler.invoke(this, EventArgs.Empty);
    }
};

ScriptLoader.createClass('ScriptLoader', null, IDisposable);

////////////////////////////////////////////////////////////////////////////////
// ScriptHost

var ScriptHost = function ScriptHost() {
}
ScriptHost.get_isLoading = function ScriptHost$get_isLoading() {
    return ScriptHost._loading;
}
ScriptHost.add_load = function ScriptHost$add_load(value) {
    if (ScriptHost._loaded) {
        value.invoke(null, EventArgs.Empty);
    }
    else {
        ScriptHost._loadHandler = Delegate.combine(ScriptHost._loadHandler, value);
    }
}
ScriptHost.remove_load = function ScriptHost$remove_load(value) {
    ScriptHost._loadHandler = Delegate.remove(ScriptHost._loadHandler, value);
}
ScriptHost.add_unload = function ScriptHost$add_unload(value) {
    ScriptHost._unloadHandler = Delegate.combine(ScriptHost._unloadHandler, value);
}
ScriptHost.remove_unload = function ScriptHost$remove_unload(value) {
    ScriptHost._unloadHandler = Delegate.remove(ScriptHost._unloadHandler, value);
}
ScriptHost.close = function ScriptHost$close() {
    if (ScriptHost._fxScripts) {
        ScriptHost._fxScripts.dispose();
        ScriptHost._fxScripts = null;
    }
    if (ScriptHost._coreScripts) {
        ScriptHost._coreScripts.dispose();
        ScriptHost._coreScripts = null;
    }
    if (ScriptHost._unloadHandler) {
        ScriptHost._unloadHandler.invoke(null, EventArgs.Empty);
        ScriptHost._unloadHandler = null;
    }
    if (ScriptHost._windowLoadHandler) {
        window.detachEvent('onload', ScriptHost._windowLoadHandler);
        ScriptHost._windowLoadHandler = null;
    }
    if (ScriptHost._windowUnloadHandler) {
        window.detachEvent('onunload', ScriptHost._windowUnloadHandler);
        ScriptHost._windowUnloadHandler = null;
    }
}
ScriptHost.initialize = function ScriptHost$initialize(coreScriptURLs, fxScriptURLs) {
    if (!ScriptHost._initialized) {
        ScriptHost._initialized = true;
        if (ScriptHost._windowLoadHandler) {
            window.detachEvent('onload', ScriptHost._windowLoadHandler);
            ScriptHost._windowLoadHandler = null;
        }
        if ((coreScriptURLs) && (coreScriptURLs.length)) {
            ScriptHost._coreScripts = new ScriptLoader(coreScriptURLs);
        }
        if ((fxScriptURLs) && (fxScriptURLs.length)) {
            ScriptHost._fxScripts = new ScriptLoader(fxScriptURLs);
        }
        if ((!ScriptHost._coreScripts) && (!ScriptHost._fxScripts)) {
            ScriptHost._onLoaded();
        }
        else {
            ScriptHost._loadScripts();
        }
    }
}
ScriptHost._loadScripts = function ScriptHost$_loadScripts() {
    ScriptHost._loading = true;
    if (ScriptHost._coreScripts) {
        ScriptHost._coreScripts.load(false, 20 * 1000, Delegate.create(null, ScriptHost._onScriptsCompleted), Delegate.create(null, ScriptHost._onScriptsError));
        return;
    }
    else {
        ScriptHost._fxScripts.load(true, 20 * 1000, Delegate.create(null, ScriptHost._onScriptsCompleted), Delegate.create(null, ScriptHost._onScriptsError));
    }
}
ScriptHost._onLoaded = function ScriptHost$_onLoaded() {
    ScriptHost._loaded = true;
    if (window.main) {
        window.main();
    }
    if (ScriptHost._loadHandler) {
        ScriptHost._loadHandler.invoke(null, EventArgs.Empty);
        ScriptHost._loadHandler = null;
    }
}
ScriptHost._onScriptsCompleted = function ScriptHost$_onScriptsCompleted(sender, e) {
    if (sender == ScriptHost._coreScripts) {
        ScriptHost._coreScripts.dispose();
        ScriptHost._coreScripts = null;
        if (ScriptHost._fxScripts) {
            ScriptHost._fxScripts.load(true, 20 * 1000, Delegate.create(null, ScriptHost._onScriptsCompleted), Delegate.create(null, ScriptHost._onScriptsError));
            return;
        }
    }
    else {
        ScriptHost._fxScripts.dispose();
        ScriptHost._fxScripts = null;
    }
    ScriptHost._loading = false;
    ScriptHost._onLoaded();
}
ScriptHost._onScriptsError = function ScriptHost$_onScriptsError(sender, e) {
}
ScriptHost._onWindowLoad = function ScriptHost$_onWindowLoad() {
    ScriptHost.initialize(null, null);
}
ScriptHost._onWindowUnload = function ScriptHost$_onWindowUnload() {
    ScriptHost.close();
}

ScriptHost.createClass('ScriptHost');

ScriptHost._coreScripts = null;
ScriptHost._fxScripts = null;
ScriptHost._loadHandler = null;
ScriptHost._unloadHandler = null;
ScriptHost._initialized = false;
ScriptHost._loading = false;
ScriptHost._loaded = false;
ScriptHost._windowLoadHandler = Delegate.create(null, ScriptHost._onWindowLoad);
ScriptHost._windowUnloadHandler = Delegate.create(null, ScriptHost._onWindowUnload);
window.attachEvent('onload', ScriptHost._windowLoadHandler);
window.attachEvent('onunload', ScriptHost._windowUnloadHandler);
