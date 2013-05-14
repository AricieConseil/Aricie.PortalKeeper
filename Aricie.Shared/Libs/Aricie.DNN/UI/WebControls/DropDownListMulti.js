/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("Aricie.DNN");

Aricie.DNN.DropDownListMulti = function (element) {
    Aricie.DNN.DropDownListMulti.initializeBase(this, [element]);
    //this._checkBoxes = false;
    this._tbClientId = null;
    this._grdClientId = null;
    this._hfValueClientId = null;
    this._AutoPostBack = null;
    this._ClientId = null;
}

Aricie.DNN.DropDownListMulti.prototype = {
    initialize: function () {
        Aricie.DNN.DropDownListMulti.callBaseMethod(this, 'initialize');
        var currentDdl = this;
        var mouse_is_inside = false;

        jQuery('#' + this.get_tbClientId()).click(function () {
            var grd = jQuery('#' + currentDdl.get_grdClientId());
            grd.toggle();
            grd.parent().toggle();

        });

        //if (currentDdl.get_AutoPostBack() == false) {
        jQuery('#' + currentDdl.get_grdClientId() + ' tr').each(function () {
            var trItem = jQuery(this);
            trItem.attr('onclick2', trItem.attr('onclick'));
            trItem.removeAttr('onclick');
        })
        jQuery('#' + currentDdl.get_grdClientId() + ' tr').click(function (e) {
            var trItem = jQuery(this);
            var lstHF = trItem.find("input[type='hidden']");
            //alert(jQuery(lstHF[0]).val());
            jQuery('#' + currentDdl.get_hfValueClientId()).val(jQuery(lstHF[0]).val());
            // alert(jQuery(lstHF[1]).val());
            jQuery('#' + currentDdl.get_tbClientId()).val(jQuery(lstHF[1]).val());
            var grd = jQuery('#' + currentDdl.get_grdClientId());
            grd.toggle();
            grd.parent().toggle();
            eval(trItem.attr('onclick2'));
        });

        /*test */
        jQuery('#' + currentDdl.get_grdClientId()).hover(function () {
            mouse_is_inside = true;
        }, function () {
            mouse_is_inside = false;
        });

        jQuery("body").mouseup(function () {
            if (!mouse_is_inside) { var grd = jQuery('#' + currentDdl.get_grdClientId()); grd.hide(); grd.parent().hide(); }
        });

    },
    dispose: function () {
        //Ajouter ici des actions dispose personnalisées
        Aricie.DNN.DropDownListMulti.callBaseMethod(this, 'dispose');
    },
    get_tbClientId: function () { return this._tbClientId; },
    set_tbClientId: function (value) { this._tbClientId = value; },

    get_grdClientId: function () { return this._grdClientId; },
    set_grdClientId: function (value) { this._grdClientId = value; },

    get_hfValueClientId: function () { return this._hfValueClientId; },
    set_hfValueClientId: function (value) { this._hfValueClientId = value; },

    get_AutoPostBack: function () { return this._AutoPostBack; },
    set_AutoPostBack: function (value) { this._AutoPostBack = value; },

    get_ClientId: function () { return this._ClientId; },
    set_ClientId: function (value) { this._ClientId = value; }

}
Aricie.DNN.DropDownListMulti.registerClass('Aricie.DNN.DropDownListMulti', Sys.UI.Control);


//if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();