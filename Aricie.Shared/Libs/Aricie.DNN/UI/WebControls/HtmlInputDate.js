/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("Aricie.DNN");

Aricie.DNN.HtmlInputDate = function (element) {
    Aricie.DNN.HtmlInputDate.initializeBase(this, [element]);
    this._clientId = null;
    this._trigger = null;
    this._min = null;
    this._max = null;
    this._format = null;
    this._currentDate = null;
    /* this._valueYear = null;
    this._valueMonth = null;
    this._valueDay = null;*/
    this._linkedDateId = null;
    this._selectors = null;
    this._yearRange = '[-5,5]';
    this._urljQueryTools = null;
    this._urlCDNjQueryTools = null;
    this._language = null;
}

Aricie.DNN.HtmlInputDate.prototype = {
    initialize: function () {
        Aricie.DNN.HtmlInputDate.callBaseMethod(this, 'initialize');
        var myDateInput = this;
        var urlCDN = this._urlCDNjQueryTools;

        //        var fileref = document.createElement('script')
        //        fileref.setAttribute("type", "text/javascript")
        //        fileref.setAttribute("src", urlCDN)
        //        document.getElementsByTagName("head")[0].appendChild(fileref)
        var options = jQuery.extend(options || {}, {
            dataType: "script",
            cache: true,
            url: urlCDN
        });
        jQuery.ajax(options).done(function () {
            if (typeof jQuery.tools == 'undefined') {
                //chargement de jquery.tools
                var urlScript = this._urljQueryTools;
                jQuery.getScript(urlScript, function (script, textStatus) {

                    myDateInput.initComponent();
                });
            }
            else { myDateInput.initComponent() }
        }
        ).fail(function (jqxhr, settings, exception) {

            var urlScript = this._urljQueryTools;
            jQuery.getScript(urlScript, function () {

                myDateInput.initComponent();
            });
        }
        );
        //        if (typeof jQuery.tools == 'undefined') {
        //            //chargement de jquery.tools
        //            var urlScript = this._urljQueryTools;
        //            jQuery.getScript(urlScript, function () {

        //                myDateInput.initComponent();
        //            });
        //        }
        //        else { myDateInput.initComponent() }



    },
    dispose: function () {
        //Add custom dispose actions here
        Aricie.DNN.HtmlInputDate.callBaseMethod(this, 'dispose');
    },
    initComponent: function () {
        jQuery.tools.dateinput.localize('fr', {
            months: 'Janvier,Fevrier,Mars,Avril,Mai,Juin,Juillet,Aout,Septembre,Octobre,Novembre,Decembre',
            shortMonths: 'jan,fev,mar,avr,mai,jun,jul,aou,sep,oct,nov,dec',
            days: 'dimanche,lundi,mardi,mercredi,jeudi,vendredi,samedi',
            shortDays: 'dim,lun,mar,mer,jeu,ven,sam'
        });


        var myDateInput = this;
        //jQuery.tools.dateinput.conf.lang = myDateInput.get_language();
        // Add custom initialization here
        //  jQuery(document).ready(function() {

        jQuery('#' + myDateInput.get_clientId()).dateinput({
            format: myDateInput.get_format(),
            selectors: myDateInput.get_selectors(),
            trigger: myDateInput.get_trigger(),
            min: myDateInput.get_min(),
            max: myDateInput.get_max(),
            lang: myDateInput.get_language(),
            yearRange: eval(myDateInput.get_yearRange()),
            css: { root: 'calroot' + myDateInput.get_clientId() }
        });

        //var currentDateInputCt = jQuery('#' + myDateInput.get_clientId());
        jQuery('#' + 'calroot' + myDateInput.get_clientId()).addClass("calroot");

        jQuery('#' + myDateInput.get_clientId()).change(function (event) {
            event.target = event.currentTarget;
            event.srcElement = event.currentTarget;
            return false;
        });
        if (myDateInput.get_currentDate() != null && myDateInput.get_currentDate().getFullYear() > 1) {
            jQuery('#' + myDateInput.get_clientId()).data('dateinput').setValue(myDateInput.get_currentDate().getFullYear(), myDateInput.get_currentDate().getMonth(), myDateInput.get_currentDate().getDate());
        }
        //});

        if (myDateInput.get_linkedDateId() != null) {

            jQuery('#' + myDateInput.get_clientId()).data('dateinput').change(function () {
                // we use it's value for the seconds input min option"
                jQuery('#' + myDateInput.get_linkedDateId()).data('dateinput').setMin(this.getValue(), true);
                //  jQuery('#' + myDateInput.get_linkedDateId()).attr('min', this.getValue());
            });


        }
    }
    ,
    get_clientId: function () {
        return this._clientId;
    },
    set_clientId: function (value) {
        this._clientId = value;
    },
    get_trigger: function () {
        return this._trigger;
    },
    set_trigger: function (value) {
        this._trigger = value;
    },
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
    }
     ,
    get_format: function () {
        return this._format;
    },
    set_format: function (value) {
        this._format = value;
    }
     ,
    get_currentDate: function () {
        return this._currentDate;
    },
    set_currentDate: function (value) {
        this._currentDate = value;
    },
    get_selectors: function () {
        return this._selectors;
    },
    set_selectors: function (value) {
        this._selectors = value;
    },
    get_yearRange: function () {
        return this._yearRange;
    },
    set_yearRange: function (value) {
        this._yearRange = value;
    },
    get_urljQueryTools: function () {
        return this._urljQueryTools;
    },
    set_urljQueryTools: function (value) {
        this._urljQueryTools = value;
    },
    get_urlCDNjQueryTools: function () { return this._urlCDNjQueryTools; },
    set_urlCDNjQueryTools: function (value) { this._urlCDNjQueryTools = value }
    /*,
    get_valueYear: function() {
    return this._valueYear;
    },
    set_valueYear: function(value) {
    this._valueYear = value;
    },
    get_valueMonth: function() {
    return this._valueMonth;
    },
    set_valueMonth: function(value) {
    this._valueMonth = value;
    },
    get_valueDay: function() {
    return this._valueDay;
    },
    set_valueDay: function(value) {
    this._valueDay = value;
    }*/,
    get_linkedDateId: function () {
        return this._linkedDateId;
    },
    set_linkedDateId: function (value) {
        this._linkedDateId = value;
    }
    , get_language: function () {
        return this._language;
    },
    set_language: function (value) {
        this._language = value;
    }
}
Aricie.DNN.HtmlInputDate.registerClass('Aricie.DNN.HtmlInputDate', Sys.UI.Control);

//if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();