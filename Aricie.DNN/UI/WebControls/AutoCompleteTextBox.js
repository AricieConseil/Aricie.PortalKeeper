/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("Aricie.DNN");

Aricie.DNN.AutoCompleteTextBox = function (element) {
    Aricie.DNN.AutoCompleteTextBox.initializeBase(this, [element]);
    this._ClientId = null;
    this._TbClientId = null;
    this._HfClientId = null;
    this._UrlWS = null;
    this._AdditionalFunctionForWSResponse = null;
    this._AdditionalSelectFunction = null;
    this._AdditionalOnClickFunction = null;
    this._EmptyText = null;
    this._AdditionalParam = null;
}

Aricie.DNN.AutoCompleteTextBox.prototype = {
    initialize: function () {
        Aricie.DNN.AutoCompleteTextBox.callBaseMethod(this, 'initialize');
        var myTextBox = this;
        myTextBox.initComponent();
    },
    dispose: function () {
        //Add custom dispose actions here
        Aricie.DNN.AutoCompleteTextBox.callBaseMethod(this, 'dispose');
    },
    initComponent: function () {
        var myTBAutoComplete = this;
        jQuery("#" + myTBAutoComplete.get_TbClientId()).click(function () {
            var emptyBox = myTBAutoComplete.get_EmptyText();
            if (jQuery("#" + myTBAutoComplete.get_TbClientId()).val() == emptyBox) {
                jQuery("#" + myTBAutoComplete.get_TbClientId()).val("");
                jQuery("#" + myTBAutoComplete.get_HfClientId()).attr("value", "");
            }
            else {
                var actualLabel = jQuery("#" + myTBAutoComplete.get_TbClientId()).val();
                var AdditionalOnClickFunction = myTBAutoComplete.get_AdditionalOnClickFunction();
                if (AdditionalOnClickFunction != null) {
                    var strParam = actualLabel;
                    var fn = window[AdditionalOnClickFunction];
                    actualLabel = fn(strParam);
                    jQuery("#" + myTBAutoComplete.get_TbClientId()).val(actualLabel);
                }
            }
        });
        jQuery("#" + myTBAutoComplete.get_TbClientId()).autocomplete({
            source: function (request, response) {

                var additionalParameter = '';

                if (myTBAutoComplete.get_AdditionalParam() != null) {
                    additionalParameter = ',"AdditionalParam":"' + myTBAutoComplete.get_AdditionalParam() + '"';
                }

                jQuery.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: myTBAutoComplete.get_UrlWS(),
                    dataType: "json",
                    data: '{"searchText":"' + request.term + '"' + additionalParameter + '}',
                    success: function (data) {
                        var objects = null;
                        if (typeof JSON != "undefined") {
                            objects = JSON.parse(data.d);
                        }
                        else {
                            objects = eval('(' + data.d + ')');
                        }

                        /*jQuery.map(objects, function (item) {
                        return {
                        label: item.name,
                        value: item.value
                        }
                        })*/
                        response(objects);

                        var AdditionalFunctionForWSResponse = myTBAutoComplete.get_AdditionalFunctionForWSResponse();
                        if (AdditionalFunctionForWSResponse != null) {
                            window[AdditionalFunctionForWSResponse]();
                        }
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                /*log(ui.item ?
                "Selected: " + ui.item.label :
                "Nothing selected, input was " + this.value);*/
                event.preventDefault();

                var toDisplay = ui.item.label;
                var AdditionalSelectFunction = myTBAutoComplete.get_AdditionalSelectFunction();
                if (AdditionalSelectFunction != null) {
                    var strParam = toDisplay;
                    var fn = window[AdditionalSelectFunction];
                    toDisplay = fn(strParam, ui.item.value);
                }
                jQuery("#" + myTBAutoComplete.get_TbClientId()).val(toDisplay);
                jQuery("#" + myTBAutoComplete.get_HfClientId()).attr("value", ui.item.value);
            },
            open: function () {
                jQuery(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                jQuery(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    },
    get_ClientId: function () {
        return this._ClientId;
    },
    set_ClientId: function (value) {
        this._ClientId = value;
    },
    get_TbClientId: function () {
        return this._TbClientId;
    },
    set_TbClientId: function (value) {
        this._TbClientId = value;
    },
    get_HfClientId: function () {
        return this._HfClientId;
    },
    set_HfClientId: function (value) {
        this._HfClientId = value;
    },
    get_UrlWS: function () {
        return this._UrlWS;
    },
    set_UrlWS: function (value) {
        this._UrlWS = value;
    },
    get_AdditionalFunctionForWSResponse: function () {
        return this._AdditionalFunctionForWSResponse;
    },
    set_AdditionalFunctionForWSResponse: function (value) {
        this._AdditionalFunctionForWSResponse = value;
    },
    get_AdditionalSelectFunction: function () {
        return this._AdditionalSelectFunction;
    },
    set_AdditionalSelectFunction: function (value) {
        this._AdditionalSelectFunction = value;
    },
    get_AdditionalOnClickFunction: function () {
        return this._AdditionalOnClickFunction;
    },
    set_AdditionalOnClickFunction: function (value) {
        this._AdditionalOnClickFunction = value;
    },
    get_AdditionalParam: function () {
        return this._AdditionalParam;
    },
    set_AdditionalParam: function (value) {
        this._AdditionalParam = value;
    },
    get_EmptyText: function () {
        return this._EmptyText;
    },
    set_EmptyText: function (value) {
        this._EmptyText = value;
    }

}
Aricie.DNN.AutoCompleteTextBox.registerClass('Aricie.DNN.AutoCompleteTextBox', Sys.UI.Control);

//if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();