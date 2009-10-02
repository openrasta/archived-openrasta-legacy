// Script# Framework
// Copyright (c) 2007, Nikhil Kothari. All Rights Reserved.
// http://projects.nikhilk.net
//


Type.createNamespace('ScriptFX.Net');

////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net._scriptResponse

ScriptFX.Net._scriptResponse = function ScriptFX_Net__scriptResponse(request, data) {
    this._timeStamp = new Date();
    this._request = request;
    this._data = data;
}
ScriptFX.Net._scriptResponse.prototype = {
    _request: null,
    _timeStamp: null,
    _data: null,
    _xml: null,
    
    get_contentLength: function ScriptFX_Net__scriptResponse$get_contentLength() {
        return 0;
    },
    
    get_contentType: function ScriptFX_Net__scriptResponse$get_contentType() {
        return 'text/javascript';
    },
    
    get_headers: function ScriptFX_Net__scriptResponse$get_headers() {
        return {};
    },
    
    get_request: function ScriptFX_Net__scriptResponse$get_request() {
        return this._request;
    },
    
    get_statusCode: function ScriptFX_Net__scriptResponse$get_statusCode() {
        return ScriptFX.Net.HTTPStatusCode.OK;
    },
    
    get_statusText: function ScriptFX_Net__scriptResponse$get_statusText() {
        return String.Empty;
    },
    
    get_timeStamp: function ScriptFX_Net__scriptResponse$get_timeStamp() {
        return this._timeStamp;
    },
    
    getHeader: function ScriptFX_Net__scriptResponse$getHeader(name) {
        return null;
    },
    
    getObject: function ScriptFX_Net__scriptResponse$getObject() {
        return this._data;
    },
    
    getText: function ScriptFX_Net__scriptResponse$getText() {
        return Type.safeCast(this._data, String);
    },
    
    getXML: function ScriptFX_Net__scriptResponse$getXML() {
        if (!this._xml) {
            var text = this.getText();
            if (text) {
                try {
                    this._xml = XMLDocumentParser.parse(text);
                }
                catch ($e1) {
                }
            }
        }
        return this._xml;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Net.ScriptTransport

ScriptFX.Net.ScriptTransport = function ScriptFX_Net_ScriptTransport(request) {
    ScriptFX.Net.ScriptTransport.constructBase(this, [ request ]);
}
ScriptFX.Net.ScriptTransport.prototype = {
    _scriptElement$1: null,
    _callbackExport$1: null,
    
    abort: function ScriptFX_Net_ScriptTransport$abort() {
        if (this._callbackExport$1) {
            Delegate.clearExport(this._callbackExport$1);
            this._callbackExport$1 = null;
        }
        if (this._scriptElement$1) {
            document.body.removeChild(this._scriptElement$1);
            this._scriptElement$1 = null;
        }
    },
    
    dispose: function ScriptFX_Net_ScriptTransport$dispose() {
        this.abort();
    },
    
    invoke: function ScriptFX_Net_ScriptTransport$invoke() {
        Debug.assert(this.getMethod() === 'GET');
        Debug.assert(!this.get_request().get_hasHeaders());
        Debug.assert(!this.get_request().get_hasCredentials());
        var callbackParam = null;
        var parameters = this.get_parameters();
        if (parameters) {
            callbackParam = parameters['callbackParameterName'];
        }
        if (!callbackParam) {
            callbackParam = 'callback';
        }
        var callback = Delegate.create(this, this._onDataAvailable$1);
        this._callbackExport$1 = Delegate.createExport(callback);
        var callbackCode = callbackParam + '=Delegate.' + this._callbackExport$1;
        var url = this.get_request().get_URI();
        if (url.indexOf('?') >= 0) {
            url += '&' + callbackCode;
        }
        else {
            url += '?' + callbackCode;
        }
        this._scriptElement$1 = document.createElement('SCRIPT');
        this._scriptElement$1.type = 'text/javascript';
        this._scriptElement$1.src = url;
        document.body.appendChild(this._scriptElement$1);
    },
    
    _onDataAvailable$1: function ScriptFX_Net_ScriptTransport$_onDataAvailable$1(data) {
        if (this._scriptElement$1) {
            document.body.removeChild(this._scriptElement$1);
            this._scriptElement$1 = null;
            this._callbackExport$1 = null;
            this.onCompleted(new ScriptFX.Net._scriptResponse(this.get_request(), data));
        }
    }
}


ScriptFX.Net._scriptResponse.createClass('ScriptFX.Net._scriptResponse', null, ScriptFX.Net.IHTTPResponse);
ScriptFX.Net.ScriptTransport.createClass('ScriptFX.Net.ScriptTransport', ScriptFX.Net.HTTPTransport);

// ---- Do not remove this footer ----
// Generated using Script# v0.5.1.0 (http://projects.nikhilk.net)
// -----------------------------------
