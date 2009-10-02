// Script# Framework
// Copyright (c) 2006, Nikhil Kothari. All Rights Reserved.
// http://projects.nikhilk.net
//


Type.createNamespace('ScriptFX.Reflection');

////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.MemberInfoType

ScriptFX.Reflection.MemberInfoType = function() { };
ScriptFX.Reflection.MemberInfoType.prototype = {
    Field: 0, 
    Method: 1, 
    Property: 2, 
    Event: 3
}
ScriptFX.Reflection.MemberInfoType.createEnum('ScriptFX.Reflection.MemberInfoType', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.TypeInfoType

ScriptFX.Reflection.TypeInfoType = function() { };
ScriptFX.Reflection.TypeInfoType.prototype = {
    Class: 0, 
    Interface: 1, 
    Enumeration: 2, 
    FlagsEnumeration: 3
}
ScriptFX.Reflection.TypeInfoType.createEnum('ScriptFX.Reflection.TypeInfoType', false);


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.ParameterInfo

ScriptFX.Reflection.ParameterInfo = function ScriptFX_Reflection_ParameterInfo(name) {
    this._name = name;
}
ScriptFX.Reflection.ParameterInfo.prototype = {
    _name: null,
    
    get_name: function ScriptFX_Reflection_ParameterInfo$get_name() {
        return this._name;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.FieldInfo

ScriptFX.Reflection.FieldInfo = function ScriptFX_Reflection_FieldInfo(name, fieldType, isStatic) {
    ScriptFX.Reflection.FieldInfo.constructBase(this, [ ScriptFX.Reflection.MemberInfoType.Field, name, fieldType, isStatic ]);
}
ScriptFX.Reflection.FieldInfo.prototype = {
    _value$1: null,
    
    get_value: function ScriptFX_Reflection_FieldInfo$get_value() {
        return this._value$1;
    },
    
    _initialize: function ScriptFX_Reflection_FieldInfo$_initialize(value) {
        this._value$1 = value;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.Reflector

ScriptFX.Reflection.Reflector = function ScriptFX_Reflection_Reflector() {
}
ScriptFX.Reflection.Reflector.getGlobalTypes = function ScriptFX_Reflection_Reflector$getGlobalTypes() {
    if (!ScriptFX.Reflection.Reflector._globalTypes) {
        var globalTypes = [];
        var classTypes = [ Object, Boolean, Number, String, Date, Array, Function, RegExp, StringBuilder, EventArgs ];
        var interfaceTypes = [ IDisposable, IEnumerable, IEnumerator, IArray ];
        var $enum1 = classTypes.getEnumerator();
        while ($enum1.moveNext()) {
            var type = $enum1.get_current();
            var typeInfo = new ScriptFX.Reflection.TypeInfo(null, ScriptFX.Reflection.TypeInfoType.Class, type.get_name());
            typeInfo._initialize(type);
            globalTypes.add(typeInfo);
        }
        var $enum2 = interfaceTypes.getEnumerator();
        while ($enum2.moveNext()) {
            var type = $enum2.get_current();
            var typeInfo = new ScriptFX.Reflection.TypeInfo(null, ScriptFX.Reflection.TypeInfoType.Interface, type.get_name());
            typeInfo._initialize(type);
            globalTypes.add(typeInfo);
        }
        ScriptFX.Reflection.Reflector._globalTypes = globalTypes;
    }
    return ScriptFX.Reflection.Reflector._globalTypes;
}
ScriptFX.Reflection.Reflector.getNamespaces = function ScriptFX_Reflection_Reflector$getNamespaces() {
    if (!ScriptFX.Reflection.Reflector._namespaces) {
        var namespaceTable = window.__namespaces;
        ScriptFX.Reflection.Reflector._namespaces = [];
        var $dict1 = namespaceTable;
        for (var $key2 in $dict1) {
            var entry = { key: $key2, value: $dict1[$key2] };
            ScriptFX.Reflection.Reflector._namespaces.add(new ScriptFX.Reflection.NamespaceInfo(entry.key, entry.value));
        }
    }
    return ScriptFX.Reflection.Reflector._namespaces;
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.PropertyInfo

ScriptFX.Reflection.PropertyInfo = function ScriptFX_Reflection_PropertyInfo(name, propertyType, isStatic) {
    ScriptFX.Reflection.PropertyInfo.constructBase(this, [ ScriptFX.Reflection.MemberInfoType.Property, name, propertyType, isStatic ]);
}
ScriptFX.Reflection.PropertyInfo.prototype = {
    _getAccessor$1: null,
    _setAccessor$1: null,
    
    get_getAccessor: function ScriptFX_Reflection_PropertyInfo$get_getAccessor() {
        return this._getAccessor$1;
    },
    
    get_isReadOnly: function ScriptFX_Reflection_PropertyInfo$get_isReadOnly() {
        return (!this._setAccessor$1);
    },
    
    get_setAccessor: function ScriptFX_Reflection_PropertyInfo$get_setAccessor() {
        return this._setAccessor$1;
    },
    
    _initialize: function ScriptFX_Reflection_PropertyInfo$_initialize(getAccessor, setAccessor) {
        this._getAccessor$1 = getAccessor;
        this._setAccessor$1 = setAccessor;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.EventInfo

ScriptFX.Reflection.EventInfo = function ScriptFX_Reflection_EventInfo(name, argsType, isStatic) {
    ScriptFX.Reflection.EventInfo.constructBase(this, [ ScriptFX.Reflection.MemberInfoType.Event, name, argsType, isStatic ]);
}
ScriptFX.Reflection.EventInfo.prototype = {
    _addAccessor$1: null,
    _removeAccessor$1: null,
    
    get_addAccessor: function ScriptFX_Reflection_EventInfo$get_addAccessor() {
        return this._addAccessor$1;
    },
    
    get_removeAccessor: function ScriptFX_Reflection_EventInfo$get_removeAccessor() {
        return this._removeAccessor$1;
    },
    
    _initialialize: function ScriptFX_Reflection_EventInfo$_initialialize(addAccessor, removeAccessor) {
        this._addAccessor$1 = addAccessor;
        this._removeAccessor$1 = removeAccessor;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.MethodInfo

ScriptFX.Reflection.MethodInfo = function ScriptFX_Reflection_MethodInfo(name, returnType, isStatic) {
    ScriptFX.Reflection.MethodInfo.constructBase(this, [ ScriptFX.Reflection.MemberInfoType.Method, name, returnType, isStatic ]);
}
ScriptFX.Reflection.MethodInfo.prototype = {
    _isConstructor$1: false,
    _method$1: null,
    _parameters$1: null,
    
    get_constructor: function ScriptFX_Reflection_MethodInfo$get_constructor() {
        return this._isConstructor$1;
    },
    
    get_method: function ScriptFX_Reflection_MethodInfo$get_method() {
        return this._method$1;
    },
    
    get_parameters: function ScriptFX_Reflection_MethodInfo$get_parameters() {
        return this._parameters$1;
    },
    
    _initialize: function ScriptFX_Reflection_MethodInfo$_initialize(isConstructor, method) {
        this._isConstructor$1 = isConstructor;
        this._method$1 = method;
        var sourceCode = method.toString();
        var indexOpenParen = sourceCode.indexOf('(');
        var indexCloseParen = sourceCode.indexOf(')');
        var signature = sourceCode.substring(indexOpenParen + 1, indexCloseParen).trim();
        var parameters = [];
        if (signature.length) {
            var paramNames = signature.split(',');
            for (var i = 0; i < paramNames.length; i++) {
                parameters[i] = new ScriptFX.Reflection.ParameterInfo(paramNames[i].trim());
            }
        }
        this._parameters$1 = parameters;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.MemberInfo

ScriptFX.Reflection.MemberInfo = function ScriptFX_Reflection_MemberInfo(memberType, name, type, isStatic) {
    this._memberType = memberType;
    this._name = name;
    this._type = type;
    this._isStatic = isStatic;
}
ScriptFX.Reflection.MemberInfo.prototype = {
    _memberType: 0,
    _name: null,
    _type: null,
    _isStatic: false,
    
    get_associatedType: function ScriptFX_Reflection_MemberInfo$get_associatedType() {
        return this._type;
    },
    
    get_isPublic: function ScriptFX_Reflection_MemberInfo$get_isPublic() {
        return (!this._name.startsWith('_')) && (!this._name.startsWith('$'));
    },
    
    get_isStatic: function ScriptFX_Reflection_MemberInfo$get_isStatic() {
        return this._isStatic;
    },
    
    get_memberType: function ScriptFX_Reflection_MemberInfo$get_memberType() {
        return this._memberType;
    },
    
    get_name: function ScriptFX_Reflection_MemberInfo$get_name() {
        return this._name;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.NamespaceInfo

ScriptFX.Reflection.NamespaceInfo = function ScriptFX_Reflection_NamespaceInfo(name, typeTable) {
    this._name = name;
    this._typeTable = typeTable;
}
ScriptFX.Reflection.NamespaceInfo.prototype = {
    _name: null,
    _typeTable: null,
    _types: null,
    
    get_name: function ScriptFX_Reflection_NamespaceInfo$get_name() {
        return this._name;
    },
    
    getTypes: function ScriptFX_Reflection_NamespaceInfo$getTypes() {
        if (!this._types) {
            this._types = [];
            var $dict1 = this._typeTable;
            for (var $key2 in $dict1) {
                var entry = { key: $key2, value: $dict1[$key2] };
                var typeInfo = null;
                var type = entry.value;
                if (Type.isClass(type)) {
                    typeInfo = new ScriptFX.Reflection.TypeInfo(this, ScriptFX.Reflection.TypeInfoType.Class, entry.key);
                }
                else if (Type.isInterface(type)) {
                    typeInfo = new ScriptFX.Reflection.TypeInfo(this, ScriptFX.Reflection.TypeInfoType.Interface, entry.key);
                }
                else if (Type.isEnum(type)) {
                    if (Type.isFlagsEnum(type)) {
                        typeInfo = new ScriptFX.Reflection.TypeInfo(this, ScriptFX.Reflection.TypeInfoType.FlagsEnumeration, entry.key);
                    }
                    else {
                        typeInfo = new ScriptFX.Reflection.TypeInfo(this, ScriptFX.Reflection.TypeInfoType.Enumeration, entry.key);
                    }
                }
                if (typeInfo) {
                    typeInfo._initialize(type);
                    this._types.add(typeInfo);
                }
            }
        }
        return this._types;
    }
}


////////////////////////////////////////////////////////////////////////////////
// ScriptFX.Reflection.TypeInfo

ScriptFX.Reflection.TypeInfo = function ScriptFX_Reflection_TypeInfo(parent, typeType, name) {
    this._parent = parent;
    this._typeType = typeType;
    this._name = name;
}
ScriptFX.Reflection.TypeInfo.prototype = {
    _parent: null,
    _typeType: 0,
    _type: null,
    _name: null,
    
    get_baseType: function ScriptFX_Reflection_TypeInfo$get_baseType() {
        return null;
    },
    
    get_fullName: function ScriptFX_Reflection_TypeInfo$get_fullName() {
        if (this._parent) {
            return this._parent.get_name() + '.' + this.get_name();
        }
        return this.get_name();
    },
    
    get_interfaces: function ScriptFX_Reflection_TypeInfo$get_interfaces() {
        return null;
    },
    
    get_isGlobal: function ScriptFX_Reflection_TypeInfo$get_isGlobal() {
        return (!this._parent);
    },
    
    get_isPublic: function ScriptFX_Reflection_TypeInfo$get_isPublic() {
        return (!this._name.startsWith('_')) && (!this._name.startsWith('$'));
    },
    
    get_name: function ScriptFX_Reflection_TypeInfo$get_name() {
        return this._name;
    },
    
    get_typeType: function ScriptFX_Reflection_TypeInfo$get_typeType() {
        return this._typeType;
    },
    
    _initialize: function ScriptFX_Reflection_TypeInfo$_initialize(type) {
        this._type = type;
    }
}


ScriptFX.Reflection.ParameterInfo.createClass('ScriptFX.Reflection.ParameterInfo');
ScriptFX.Reflection.MemberInfo.createClass('ScriptFX.Reflection.MemberInfo');
ScriptFX.Reflection.FieldInfo.createClass('ScriptFX.Reflection.FieldInfo', ScriptFX.Reflection.MemberInfo);
ScriptFX.Reflection.Reflector.createClass('ScriptFX.Reflection.Reflector');
ScriptFX.Reflection.PropertyInfo.createClass('ScriptFX.Reflection.PropertyInfo', ScriptFX.Reflection.MemberInfo);
ScriptFX.Reflection.EventInfo.createClass('ScriptFX.Reflection.EventInfo', ScriptFX.Reflection.MemberInfo);
ScriptFX.Reflection.MethodInfo.createClass('ScriptFX.Reflection.MethodInfo', ScriptFX.Reflection.MemberInfo);
ScriptFX.Reflection.NamespaceInfo.createClass('ScriptFX.Reflection.NamespaceInfo');
ScriptFX.Reflection.TypeInfo.createClass('ScriptFX.Reflection.TypeInfo');
ScriptFX.Reflection.Reflector._globalTypes = null;
ScriptFX.Reflection.Reflector._namespaces = null;

// ---- Do not remove this footer ----
// Generated using Script# v0.5.1.0 (http://projects.nikhilk.net)
// -----------------------------------
