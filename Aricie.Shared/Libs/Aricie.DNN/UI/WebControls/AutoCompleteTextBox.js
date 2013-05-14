/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("Aricie.DNN");

Aricie.DNN.AutoCompleteTextBox = function (element) {
    Aricie.DNN.AutoCompleteTextBox.initializeBase(this, [element]);
    this._ClientId = null;
    this._TbClientId = null;
    this._HfClientId = null;
    this._UrlWS = null;
  
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
            jQuery("#" + myTBAutoComplete.get_TbClientId()).val("");
            jQuery("#" + myTBAutoComplete.get_HfClientId()).attr("value","");
        });
        jQuery("#" + myTBAutoComplete.get_TbClientId()).autocomplete({
            source: function (request, response) {

                jQuery.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: myTBAutoComplete.get_UrlWS(),
                    dataType: "json",
                    data: '{"searchText":"' + request.term + '"}',
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
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                /*log(ui.item ?
                "Selected: " + ui.item.label :
                "Nothing selected, input was " + this.value);*/
                event.preventDefault();
                jQuery("#" + myTBAutoComplete.get_TbClientId()).val(ui.item.label);
                jQuery("#" + myTBAutoComplete.get_HfClientId()).attr("value", ui.item.value);

            },
            open: function () {
                jQuery(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                jQuery(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });


    }
    ,
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
    }

}
Aricie.DNN.AutoCompleteTextBox.registerClass('Aricie.DNN.AutoCompleteTextBox', Sys.UI.Control);

//if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();





