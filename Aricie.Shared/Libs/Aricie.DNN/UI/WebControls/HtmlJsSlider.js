/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("Aricie.DNN");

Aricie.DNN.HtmlJsSlider = function (element) {
    Aricie.DNN.HtmlJsSlider.initializeBase(this, [element]);
    //this._checkBoxes = false;
    this._minValueId = null;
    this._maxValueId = null,
    this._lbValId = null;
    this._plSliderId = null;
    this._initMin = null;
    this._initMax = null;
    this._min = null;
    this._max = null;
    this._urlJqueryUI = null;
    this._useRange = null;
}

Aricie.DNN.HtmlJsSlider.prototype = {
    initialize: function () {
        Aricie.DNN.HtmlJsSlider.callBaseMethod(this, 'initialize');
        var currentJSSlider = this;

        if (typeof jQuery.ui == 'undefined') {
            //chargement de jquery UI
            var urlScript = this._urlJqueryUI;
            jQuery.getScript(urlScript, function () {
                currentJSSlider.initComponent();
            });
        }
        else { currentJSSlider.initComponent() }

    },
    dispose: function () {
        //Ajouter ici des actions dispose personnalisées
        Aricie.DNN.HtmlJsSlider.callBaseMethod(this, 'dispose');
    }, initComponent: function () {
        var currentJSSlider = this;
        var sliderRange = jQuery("#" + currentJSSlider.get_plSliderId());
        sliderRange.slider({
            range: currentJSSlider.get_useRange(),
            min: currentJSSlider.get_initMin(),
            max: currentJSSlider.get_initMax(),
            // values: [currentJSSlider.get_min(), currentJSSlider.get_max()],
            slide: function (event, ui) {
                if (currentJSSlider.get_useRange == true) {
                    jQuery("#" + currentJSSlider.get_minValueId()).val(ui.values[0]);
                    jQuery("#" + currentJSSlider.get_maxValueId()).val(ui.values[1]);
                    jQuery("#" + currentJSSlider.get_lbValId()).html("Min :" + ui.values[0] + " <br/>Max :" + ui.values[1]);
                }
                else {
                    jQuery("#" + currentJSSlider.get_minValueId()).val(ui.value);
                    jQuery("#" + currentJSSlider.get_maxValueId()).val(ui.value);
                    jQuery("#" + currentJSSlider.get_lbValId()).html( ui.value);
                }

            }
        });
        if (currentJSSlider.get_useRange == true) {
            sliderRange.slider("option", "values", [currentJSSlider.get_min(), currentJSSlider.get_max()]);
        }
        else {
            sliderRange.slider("option", "value", currentJSSlider.get_min());
        }
        jQuery("#" + currentJSSlider.get_lbValId()).html("Min :" + sliderRange.slider("values", 0) +
			" <br/>Max :" + sliderRange.slider("values", 1));
    }

    ,
    get_minValueId: function () {
        return this._minValueId;
    },
    set_minValueId: function (value) {
        this._minValueId = value;
    },
    get_maxValueId: function () {
        return this._maxValueId;
    },
    set_maxValueId: function (value) {
        this._maxValueId = value;
    },
    get_lbValId: function () {
        return this._lbValId;
    },
    set_lbValId: function (value) {
        this._lbValId = value;
    },
    get_plSliderId: function () {
        return this._plSliderId;
    },
    set_plSliderId: function (value) {
        this._plSliderId = value;
    }
    ,
    get_initMin: function () {
        return this._initMin;
    },
    set_initMin: function (value) {
        this._initMin = value;
    }
    ,
    get_initMax: function () {
        return this._initMax;
    },
    set_initMax: function (value) {
        this._initMax = value;
    },
    get_urlJqueryUI: function () {
        return this._urlJqueryUI;
    },
    set_urlJqueryUI: function (value) {
        this._urlJqueryUI = value;
    }
     ,
    get_min: function () {
        return this._min;
    },
    set_min: function (value) {
        this._min = value;
    }
    ,
    get_max: function () {
        return this._max;
    },
    set_max: function (value) {
        this._max = value;
    },
    get_useRange: function () {
        return this._useRange;
    },
    set_useRange: function (value) { this._useRange = value; }

}


Aricie.DNN.HtmlJsSlider.registerClass('Aricie.DNN.HtmlJsSlider', Sys.UI.Control);


//if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
