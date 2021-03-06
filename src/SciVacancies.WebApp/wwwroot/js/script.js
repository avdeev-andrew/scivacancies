﻿var graphs = {}; //глобальная переменная для графиков
jQuery.extend(jQuery.expr[":"], {
    attrStartsWith: function (el, _, b) {
        for (var i = 0, atts = el.attributes, n = atts.length; i < n; i++) {
            if (atts[i].nodeName.indexOf(b[3]) === 0) {
                return true;
            }
        }

        return false;
    },
    attrValueStartsWith: function (el, _, b) {
        for (var i = 0, atts = el.attributes, n = atts.length; i < n; i++) {
            if (atts[i].nodeValue.indexOf(b[3]) === 0) {
                return true;
            }
        }

        return false;
    }
});

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
}

$(document).ready(function () {

    //dotdotdot
    $(".line_three").dotdotdot({
        /*	The text to add as ellipsis. */
        ellipsis: '... ',

        /*	How to cut off the text/html: 'word'/'letter'/'children' */
        wrap: 'word',

        /*	Wrap-option fallback to 'letter' for long words */
        fallbackToLetter: true,

        /*	jQuery-selector for the element to keep and put after the ellipsis. */
        after: null,

        /*	Whether to update the ellipsis: true/'window' */
        watch: false,

        /*	Optionally set a max-height, if null, the height will be measured. */
        height: null,

        /*	Deviation for the height-option. */
        tolerance: 0,

        /*	Callback function that is fired after the ellipsis is added,
			receives two parameters: isTruncated(boolean), orgContent(string). */
        callback: function (isTruncated, orgContent) { },

        lastCharacter: {

            /*	Remove these characters from the end of the truncated text. */
            remove: [' ', ',', ';', '.', '-', '!', '?'],

            /*	Don't add an ellipsis if this array contains 
				the last character of the truncated text. */
            noEllipsis: []
        }
    });
    $(".line_four").dotdotdot({
        /*	The text to add as ellipsis. */
        ellipsis: '... ',

        /*	How to cut off the text/html: 'word'/'letter'/'children' */
        wrap: 'word',

        /*	Wrap-option fallback to 'letter' for long words */
        fallbackToLetter: true,

        /*	jQuery-selector for the element to keep and put after the ellipsis. */
        after: null,

        /*	Whether to update the ellipsis: true/'window' */
        watch: false,

        /*	Optionally set a max-height, if null, the height will be measured. */
        height: null,

        /*	Deviation for the height-option. */
        tolerance: 0,

        /*	Callback function that is fired after the ellipsis is added,
			receives two parameters: isTruncated(boolean), orgContent(string). */
        callback: function (isTruncated, orgContent) { },

        lastCharacter: {

            /*	Remove these characters from the end of the truncated text. */
            remove: [' ', ',', ';', '.', '-', '!', '?'],

            /*	Don't add an ellipsis if this array contains 
				the last character of the truncated text. */
            noEllipsis: []
        }
    });
    $(".line_five_content").dotdotdot({
        /*	The text to add as ellipsis. */
        ellipsis: '... ',

        /*	How to cut off the text/html: 'word'/'letter'/'children' */
        wrap: 'word',

        /*	Wrap-option fallback to 'letter' for long words */
        fallbackToLetter: true,

        /*	jQuery-selector for the element to keep and put after the ellipsis. */
        after: null,

        /*	Whether to update the ellipsis: true/'window' */
        watch: false,

        /*	Optionally set a max-height, if null, the height will be measured. */
        height: null,

        /*	Deviation for the height-option. */
        tolerance: 0,

        /*	Callback function that is fired after the ellipsis is added,
			receives two parameters: isTruncated(boolean), orgContent(string). */
        callback: function (isTruncated, orgContent) { },

        lastCharacter: {

            /*	Remove these characters from the end of the truncated text. */
            remove: [' ', ',', ';', '.', '-', '!', '?'],

            /*	Don't add an ellipsis if this array contains 
				the last character of the truncated text. */
            noEllipsis: []
        }
    });
    //select
    //для регионов под графиками
    var paramsRegions = {
        changedEl: "select#RegionId",
        visRows: 6,
        scrollArrows: true
    }
    cuSel(paramsRegions);
    //select
    //
    var params = {
        changedEl: "select:not(.skip)",
        visRows: 12,
        scrollArrows: true
    }
    cuSel(params);
    //удаляет классы
    $(".cusel").click(function () {
        $(".cusel").removeClass("cuselOpen");
    });

    //jquery datepicker
    $(function () {
        $.datepicker.setDefaults(
            $.extend($.datepicker.regional["ru"])
        );
        $(".datepicker-vacancy").datepicker({
            changeMonth: true,
            changeYear: true,
            minDate: "+1D",
            showButtonPanel: true
        });
    });

    //checkbox
    $(".checkbox").not(".disabled").click(function () {
        if ($(this).parent().find("input").attr("checked")) {
            $(this).removeClass("checked");
            $(this).parent().find("input").attr("checked", false);
        } else {
            $(this).addClass("checked");
            $(this).parent().find("input").attr("checked", true);
        }
    });

    $(".radio").click(function () {
        if ($(this).hasClass("checked")) {
            return false;
        } else {
            $(this).parents(".radio-list:first").children("li").find(".radio:first").removeClass("checked");
            $(this).closest("li").siblings("li").find("input").attr("checked", false);
            $(this).find("input").attr("checked", true);
            $(this).addClass("checked");
        }
    });
    $(".radio-list label").click(function () {
        if ($(this).siblings("span").hasClass("checked")) {
            return false;
        } else {
            $(this).closest(".radio-list").find(".radio").removeClass("checked");
            $(this).closest("li").siblings("li").find("input").attr("checked", false);
            $(this).siblings("span").find("input").attr("checked", true);
            $(this).siblings("span").addClass("checked");
        }
    });
    // Popup
    //$('.window-popup, .popup-bg, .bg-window').hide(); 
    $(".popup-bg").css({ opacity: .2 });

    function alignCenter(elem) {
        var modalHeight = ($(window).height() - 40 * 2);
        elem.css({
            height: modalHeight + "px",
            left: ($(window).width() - elem.outerWidth()) / 2 + "px",
            top: /*($(window).height() - elem.outerHeight()) / 2*/ 20 + "px"
        });
        $(elem).find("div.content-popup").css({
            height: (modalHeight - 100 - 50) + "px"
        });
    }
    function alignCenterLoginPopup(elem) {
        //var modalHeight = ($(window).height() - 40 * 4);
        var modalHeight = 500;
        elem.css({
            height: modalHeight + "px",
            left: ($(window).width() - elem.outerWidth()) / 2 + "px",
            top: /*($(window).height() - elem.outerHeight()) / 2*/ 20 + "px"
        });
        $(elem).find("div.content-popup").css({
            height: (modalHeight - 100 - 50) + "px"
        });
    }

    alignCenter($(".window-popup"));
    alignCenterLoginPopup($("#login-popup"));
    $(window).resize(function () {
        alignCenter($(".window-popup"));
        alignCenterLoginPopup($("#login-popup"));
    });
    $(".open-popup").click(function () {
        var source = this;
        if ($(source).attr("data-modal") != undefined) {
            $(".window-popup[data-name=\"" + $(source).attr("data-modal") + "\"]").fadeIn(300);
        } else {
            $(".popup-bg, .window-popup, .bg-window").fadeIn(300);
        }
        return false;
    });
    $(".close-popup").click(function () {
        $(".popup-bg, .window-popup, .bg-window").fadeOut(300);
    });
    $("#cancellogin").click(function () {
        $($(this).parents("form")[0]).find("input[type!=\"submit\"]").val(null);
        $(".close-popup").click();
    });
    // function centering
    $("#subscribe").popover({
        'placement': "bottom",
        'trigger': "hover",
        html: true,
        content: function () {
            return $("#subscribe-popover").html();
        }
    });
    $("#subscribed").popover({
        'placement': "bottom",
        'trigger': "hover",
        html: true,
        content: function () {
            return $("#subscribed-popover").html();
        }
    });
    $("#lk").popover({
        'placement': "bottom",
        'trigger': "hover",
        html: true,
        content: function () {
            return $("#lk-popover").html();
        }
    });
    //закладки в личном кабинете
    $(".nav-tabs li:first a, #tab.nav-tabs li:first a").tab("show");
    $(".nav-tabs a").click(function (e) {
        e.preventDefault();
        $(this).tab("show");
    });

    //carousel
    var slider = $(".bxslider").bxSlider({
        mode: "horizontal",
        autoControls: true,
        minSlides: 1,
        pager: false,
        adaptiveHeight: true
    });

    //open block solutions
    $(".solutions").click(function () {
        $(this).parents(".content-slide").find(".b-solution").slideToggle("fast", function () {
            var current = slider.getCurrentSlide();
            /*slider.reloadSlider();
        slider.goToSlide(current);*/
            //slider.find("li").eq(current).css("height", "100%");
            $(".bx-viewport").css("height", "100%");

        });
        if ($(this).hasClass("op")) {
            $(this).parent("div").hide().next("div").show();
        } else {
            $(this).parent("div").hide().prev("div").show();
        }
    });

    /*
 *  this code need for navigate by new pager values
 */
    // changeaction
    $("input[changeaction=true]").keypress(function (e) {
        if (e.which == 13) {
            var input = this;
            var form = $(input).parents("form")[0];
            var uri = updateQueryStringParameter($(form).attr("action"), e.currentTarget.name, e.currentTarget.value);
            console.log(uri);
            window.location = uri;
        }
    });
    $("input[changeaction=true]").parents("form").submit(function (e) {
        e.preventDefault();
    });

    /*
 * показать все элементы в группе значений в поиске
 */
    $("div.filter-contents span.show-all-list").click(function () {
        var source = this;
        var parentContainer = $(source).parents('.cat-filter')[0];
        //plain checkbox list
        $(parentContainer).find("li").show(500, function () {
            $(source).hide();
            $(source).siblings("span.hide-unselected").show();
        });
    });
    /*
 * скрыть невыбранные элементы в группе значений в поиске
 */
    $("div.filter-contents span.hide-unselected").click(function () {
        var source = this;
        var parentContainer = $(source).parents('.cat-filter')[0];
        //plain checkbox list
        $(parentContainer).find("span.checkbox:not(.checked)").parents("li").hide(300, function () {
            $(source).hide();
            $(source).siblings("span.show-all-list").show();
        });
    });
    /*
 * Исследователь редактирует аккаунт: удалить интерес из списка
 */
    $(".list-tags.research-interests").find("a.link-remove").click(function () {
        var source = this;
        $($(source).parents("li")[0]).remove();
        return false;
    });
    /*
 * развернуть/свернуть список значений в фильтре
 */
    $('.collapsible-filter-header').click(function (event) {
        event.stopPropagation();
        var source = this;
        if ($(source).hasClass('open')) {
            $(source).children('ul').hide();
            $(source).removeClass('open');
        } else {
            $(source).addClass('open');
            $(source).children('ul').show();
        }
    });
    $('li.collapsible-filter-header.open').children('ul').show(); //показать вложенные элементы если добавлен класс .open
    /*
 * исправление распространения событий для списка фильтров при поиске
 */
    $('li.li-checkbox').click(function (event) {
        event.stopPropagation();
    });
    /*
 * Управление внутренними вкладками в Областях науки
 */
    $('.jshelper-sub-research-directions').click(function () {
        var source = this;

        $(source).siblings().removeClass('active');
        $(source).addClass('active');
        var parentContainer = $(source).parents('.jshelper-parent-of-tabs')[0];
        $(parentContainer).siblings('.jshelper-list-sections-science').hide();
        $(parentContainer).siblings('.jshelper-list-sections-science[id="' + $(source).attr('data-tabname') + '"]').show();
    });
    /*
 * закрывать модальное окно при нажатии кнопки Отмены
 */
    $('span.icon-close').click(function () {
        var source = this;
        var parent = $(source).parents('.window-popup')[0];
        $(parent).find('span.close-popup').click();
    });
    /*
 * редактирование Вакансии (развернуть/скрыть Критерии)
 */
    //b-publication open
    $('span.name-section').click(function () {
        var source = this;
        var parent = null;
        try {
            parent = $(source).parents('.b-publication')[0];
        } catch (e) {
        }
        if (parent != null) {
            $(parent).toggleClass('open');
        }
    });
    /*
 * показать все элементы (в Вакансии развернуть/скрыть Критерии)
 */
    $("div.lnk-container span.icon-hsm-eye").click(function () {
        var source = this;
        var parentContainer = $(source).parents('.right-cell')[0];
        $(parentContainer).find("div.b-publication").addClass('open');
        $(source).hide();
        $(source).siblings("span.icon-sm-eye").show();
    });
    /*
 * скрыть все элементы (в Вакансии развернуть/скрыть Критерии)
 */
    $("div.lnk-container span.icon-sm-eye").click(function () {
        var source = this;
        var parentContainer = $(source).parents('.right-cell')[0];
        $(parentContainer).find("div.b-publication").removeClass('open');
        $(source).hide();
        $(source).siblings("span.icon-hsm-eye").show();
    });
    ///*
    // * дизайн для кнопки выбора Фотографии
    // */
    //var buttonFile = $('a[data-selectorphoto="true"]');
    var buttonFile = document.getElementById('buttonFile');
    //var file = $('input[data-selectphoto="true"]');
    var file = document.getElementById('files');
    $(buttonFile).click(function () {
        file.click();
        file.onchange = function () {
            $("#fileName").remove();
            $(file).prev().after("<div class='italic mt10' id='fileName'>" + file.value + "</div>");
        }
        return false;
    });

    /*
         * переключатели для главной страницы
         */
    function toggleChevron(e) {
        $(e.target).prev('.panel-heading').parent().toggleClass('selected');
    }

    $('#accordion').on('hidden.bs.collapse', toggleChevron);
    $('#accordion').on('show.bs.collapse', toggleChevron);

    $('.tabs-toggle__item:first a').tab('show');
    $('.tabs-toggle__item a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
    });
    /*
 * «срок трудового договора» поля должны показываться только, если выбираешь «срочный»
 */
    $('#ContractTypeValue').change(function () {
        toggleContractTime();
    });
    toggleContractTime();
    /*
 * сброс фильтра
 */
    $(".filter-link-uncheck-all").click(function () {
        var source = this;
        var parent = $(source).parents('.filter-contents')[0];

        $(parent).find('input[type="checkbox"]').attr('checked', false);
        $(parent).find('span.checkbox.checked').removeClass('checked');

        $(parent).find('input[type="text"]').val(null);

        //$(parent).parents('form').submit();
    });
    /*
     * сброс фильтра
     */
    $(".filter-link-check-all").click(function () {
        var source = this;
        var parent = $(source).parents('.filter-contents')[0];

        $(parent).find('input[type="checkbox"]').attr('checked', true);
        $(parent).find('span.checkbox').not('.checked').addClass('checked');

        $(parent).find('input[type="text"]').val(null);

        //$(parent).parents('form').submit();
    });
    /*
     * Временно для переключателя
     */
    $(".tabs_after_title > li").click(function () {
        $(".tabs_after_title > li").siblings().removeClass("active");
        $(this).addClass("active");
    });
    /*
     *На форме редактирования списков добавить объект 
     */
    $('.property-list-container').each(function () {
        var parentDiv = this;
        var prefix = $(parentDiv).attr('data-property-responsible');

        if (prefix !== undefined && prefix != null) {
            var count = $(parentDiv).find(":visible.property-list-item").length;
            if (count === 0) {
                addingNewItemToList(parentDiv, prefix);
            }
        }
    });
    /*
    end of the code
    */
});

/*
 *для областей науки вывести количество вакансий и среднюю зарплату 
 */
function getResearchDirectionsStatistics(childId) {
    $("[aria-labelledby='heading_"+childId+"']").find('div.item-direction[data-researchdirectionid]').each(function () {
        var id = $(this).attr('data-researchdirectionid');

        $.ajax({
            url: "/analytics/countbyresearchdirection/" + id,
            success: function (data) {
                var parentDiv = $('div.item-direction[data-researchdirectionid="' + data.Id + '"]');
                $(parentDiv).find("span.totalvacancies").html(data.Count);
                $(parentDiv).find("span.averagesalaries").html(data.AverageSalary);
            }
        });

    });

    $("[aria-controls='collapse_" + childId + "']").removeAttr("onclick");
}

/*
 * обработка каскадных выпадающих списков для Cusel (год окончания периода не может быть меньше года начала периода)
 */
function cuselValueChanged(source, key) {
    var newValue = $(source).val();
    //console.log(key + ' - ' + newValue);

    //найти Зависимый элемент
    var children = $('[data-cusel-number-child="' + key + '"');
    if (children != undefined) {
        //если есть Зависимый
        //console.log(children);

        var children_cuselText = $(children).find('.cuselText');
        var childrenCurrentValue = children_cuselText[0].innerText;
        if (childrenCurrentValue != undefined) {
            //console.log(childrenCurrentValue);

            if (childrenCurrentValue < newValue)
                //если если значение Зависимого меньше значения Родительского
            {
                //то поставить родительское значение как новое значение Зависимого
                //console.log("it is less then parent");
                $(children_cuselText).html(newValue);

                $(children).find('.cusel-scroll-pane').find('span.cuselActive').removeClass('cuselActive');
                $(children).find('.cusel-scroll-pane').find('span[val="' + newValue + '"]').addClass('cuselActive');
            }
            /*
            //урезать (скрыть) неподходящие значения потомков
            //обновить вид потомка
            */
        }

    }
}
/*
 * Графики. запросить новые данные
 */
function refreshAllGraphicData() {
    graphs = graphs || {}; //глобальная переменная для графиков    

    if (graphs.chart === undefined || graphs.chart2 === undefined)
    { return; }

    //refreshData
    graphs.regionId = $('#RegionId').val();

    graphs.period = $('#graphicPeriod').find('.active').attr('data-pariod-value');
    //end: refreshData


    if (graphs.chart !== null) {
        var dataGraph1 = {
            regionId: graphs.regionId,
            interval: graphs.period
        };
        graphs.chart.options.data = [];
        graphs.chart.render();
        $.get("/analytics/VacancyPositions", dataGraph1, function (data) {
            graphs.chart.options.data = data; // Set Array of dataSeries
            graphs.chart.render();
        });
    }

    if (graphs.chart2 != null) {
        var dataGraph2 = {
            regionId: graphs.regionId,
            interval: graphs.period
        };
        graphs.chart2.options.data = [];
        graphs.chart2.render();
        $.get("/analytics/VacancyPayments", dataGraph2, function (data) {
            graphs.chart2.options.data = data; // Set Array of dataSeries
            graphs.chart2.render();
        });
    }
}
/*
 * каптча
 */
function reloadImg(captchaImageFieldName) {
    var d = new Date();
    $('#' + captchaImageFieldName).attr("src", "/captcha/fetch?w=164&h=50" + d.getTime());
    $('#Captcha').val('');
}
/*
 * сделать некоторую обработку перед отправкой формы Регистрации
 */
function beforeFormSubmitRegister(source, captchaImageFieldName, captchaInputFieldName, captchaEmptyFieldName, captchaInvalidFieldName, event) {
    var form = $('#' + captchaImageFieldName).parents('form')[0];
    if (!$(form).hasClass('validated')) {
        event.preventDefault();


        var validData = {
            captchaText: $('#' + captchaInputFieldName).val()
        };

        $('#' + captchaInvalidFieldName).hide();
        if (validData.captchaText === undefined || validData.captchaText === null || validData.captchaText === "") {
            $('#' + captchaEmptyFieldName).show();
            return false;
        }
        $('#' + captchaEmptyFieldName).hide();

        $.ajax({
            url: '/captcha/isvalid',
            type: 'POST' /*'GET'*/,
            data: validData,
            success: function (isCaptureValid) {
                if (isCaptureValid) {
                    $('#' + captchaInvalidFieldName).hide();
                    $(form).addClass('validated');
                    $(form).submit();
                    return true;
                } else {
                    reloadImg(captchaImageFieldName);
                    $('#' + captchaInvalidFieldName).show();
                    return false;
                }
            },
            error: function (error) {
                reloadImg(captchaImageFieldName);
                $('#' + captchaInvalidFieldName).show();
                return false;
            }
        });
    }
};
/*
 * на форме Регистрации регистрации нажатие по кнопке "Согласие на обработку персональным данных"
 */
function vacancySaveOptions(options) {
    if (options.publish !== undefined && options.publish) {
        $("form").find("input[type=\"hidden\"][name=\"ToPublish\"]").val(true);
        return true;
    }
    if (options.saveDraft !== undefined && options.saveDraft) {
        $("form").find("input[type=\"hidden\"][name=\"ToPublish\"]").val(false);
        return true;
    }
    return false;
};

function updateQueryStringParameter(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf("?") !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, "$1" + key + "=" + value + "$2");
    }
    else {
        return uri + separator + key + "=" + value;
    }
}
/*
 * Выбрать значение из словаря
 */
function selectedItemFromModalDictionary(hiddenInputName, textInput, newValue, displayText) {
    $("#" + hiddenInputName).val(newValue);
    $("#" + textInput).val(displayText);
    $("[data-setnewvalue=\"" + textInput + "\"]").val(displayText);
    $("div[data-name=\"" + hiddenInputName + "\"]").find(".close-popup").click();
    return false;
};
/**
 * перед отправкой формы удалить шаблоны пополнения списков
 */
function isEmptyOrSpaces(str) {
    return str === undefined || str === null || str === 0 || str === "0" || str.match(/^ *$/) !== null;
}
function checkEmptyProperties(form, arrayProperties)
{
    var itemsToDelete = [];
    //проверяем каждое свойство-коллекцию
    $(arrayProperties).each(function (index) {
        var arrayProperty = arrayProperties[index];
        var container = $(form).find('.property-list-container[data-property-responsible="' + arrayProperty.name + '"]');
        var subForms = $(container).find('.property-list-item');

        if (arrayProperty.fieldsCouldBeEmpty.length > 0) {

            //проверяем каждый объект в свойстве-коллекции
            for (var i = 0; i < subForms.length; i++) {
                var subForm = subForms[i];
                var counterOfEmptyFields = 0;
                //проверяем, что все свойства в суб-форме, которые могут быть пустыми, - пустые
                for (var fieldNameIndex = 0; fieldNameIndex < arrayProperty.fieldsCouldBeEmpty.length; fieldNameIndex++) {
                    var fieldName = arrayProperty.fieldsCouldBeEmpty[fieldNameIndex];
                    var fieldValueInObjectInArray = $(subForm).find("input[name^='" + arrayProperty.name + "'][name$='" + fieldName + "']").val();
                    if (isEmptyOrSpaces(fieldValueInObjectInArray)) {
                        counterOfEmptyFields++;
                    }
                }
                //если все возможные поля в суб-форме пустые, то удалить суб-форму
                if (counterOfEmptyFields === arrayProperty.fieldsCouldBeEmpty.length) {
                    itemsToDelete.push(subForm);
                }
            }
        } else {

            //проверяем каждый объект в свойстве-коллекции
            for (var i = 0; i < subForms.length; i++) {
                var subForm = subForms[i];
                var counterOfNonEmptyFields = 0;
                var inputsInSubForm = $(subForm).find("input:not(:hidden)");
                //проверяем, что все свойства в суб-форме, которые могут быть пустыми, - пустые
                $(inputsInSubForm).each(function () {
                    var source = $(this).val();
                    if (!isEmptyOrSpaces(source)) {
                        counterOfNonEmptyFields++;
                    }
                });
                //если все возможные поля в суб-форме пустые, то удалить суб-форму
                if (counterOfNonEmptyFields === 0) {
                    itemsToDelete.push(subForm);
                }
            }
        }
    });
    $(itemsToDelete).remove();
}
function beforeFormSubmit(source, key) {
    var form = $(source).parents("form")[0];
    $(form).find("[data-list-template=\"true\"]").remove();

    //удалить незаполненные суб-формы с формы перед отправкой на сервер
    if (key !== undefined && key === 'researcherEdit') {
        //свойства-коллекции, в которых могут быть суб-формы, нужно проверить на пустые поля, и удалить эти суб-формы из списков если они не заполнены
        var arrayProperties = [
            { name: 'Educations', fieldsCouldBeEmpty: ['City', 'UniversityShortName', 'FacultyShortName'] },
            { name: 'ResearchActivity', fieldsCouldBeEmpty: ['organization', 'position', 'title'] },
            { name: 'TeachingActivity', fieldsCouldBeEmpty: ['organization', 'position', 'title'] },
            { name: 'OtherActivity', fieldsCouldBeEmpty: ['organization', 'position', 'title'] },
            { name: 'Rewards', fieldsCouldBeEmpty: ['title', 'org'] },
            { name: 'Memberships', fieldsCouldBeEmpty: ['org', 'position'] },
            { name: 'Conferences', fieldsCouldBeEmpty: ['conference', 'title'] },
            { name: 'Publications', fieldsCouldBeEmpty: ['Name', 'Authors'] }
        ];
        checkEmptyProperties(form, arrayProperties);

    }

    //удалить незаполненные суб-формы с формы перед отправкой на сервер
    if (key !== undefined && key === 'vacancyEdit') {
        //свойства-коллекции, в которых могут быть суб-формы, нужно проверить на пустые поля, и удалить эти суб-формы из списков если они не заполнены
        var arrayProperties = [
            { name: 'CustomCriterias', fieldsCouldBeEmpty: [] }
        ];
        checkEmptyProperties(form, arrayProperties);
    }

    return true;
};
/**
 * добавить новую форму к списку с объектами (редактирование профиля)
 */
function addNewItemToList(source, prefix) {
    //<div class="table-form mt15" data-innercount="@(Model.Educations.Count+1)">
    var parentDiv = $(source).parents(".property-list-container")[0];
    addingNewItemToList(parentDiv, prefix);
    return false;
};
/**
 * удалить форму из списка с объектами (редактирование профиля)
 */
function removeItemFromList(source, prefix) {
    if (confirm("Вы уверены что хотите удалить эту запись?")) {
        var parentDiv = $(source).parents(".property-list-item")[0];

        var parentDivContainer = $(source).parents(".property-list-container")[0];

        $(parentDiv).remove();

        if (prefix !== undefined && prefix != null) {
            var count = $(parentDivContainer).find(":visible.property-list-item").length;
            if (count === 0) {
                addingNewItemToList(parentDivContainer, prefix);
            }
        }

        return false;
    }
    return false;
};
/**
 * добавление формы к списку с объектами (редактирование профиля)
 */
function addingNewItemToList(parentDiv, prefix) {
    //получаем текущий индекс количества строк
    var newIndex = parseInt($(parentDiv).attr("data-innercount")) + 1;
    var oldPrefixDash = prefix + "_0__",
        oldPrefixSharp = prefix + "##0##",
        oldPrefixBracket = prefix + "\\[0\\]",
        newPrefixDash = prefix + "_" + newIndex + "__",
        newPrefixBracket = prefix + "[" + newIndex + "]",
        newPrefixSharp = prefix + newIndex;

    //находим шаблон для добавления строк
    var templateDiv = $(parentDiv).find("[data-list-template=\"true\"]").clone();
    //меняем индексы в новом шаблоне
    var templateDivInnerHtml = templateDiv
        .html()
        .replace(new RegExp(oldPrefixBracket, 'g'), newPrefixBracket)
        .replace(new RegExp(oldPrefixDash, 'g'), newPrefixDash)
        .replace(new RegExp(oldPrefixSharp, 'g'), newPrefixSharp)
        .replace(new RegExp('js_add_guid_here', 'g'), guid()) //генерим новые гуиды для работы select'ов. (по этим Guid работают "каскады" для отработки правила "окончания периода не может быть раньше его начала")
    ;
    //обновляем шаблон
    templateDiv.html(templateDivInnerHtml);

    $(templateDiv).find("select.skip").removeClass("skip").addClass("newselect");

    var lastItemRow = $(parentDiv).find(".property-list-item").last();
    $(templateDiv).removeAttr('data-list-template');
    $(templateDiv).find("input[name=\"" + prefix + '.Index"]').val(newIndex);
    $(lastItemRow).after(templateDiv);
    $(templateDiv).fadeIn("fast");

    var paramsSelect = {
        changedEl: "select.newselect",
        visRows: 12,
        scrollArrows: true
    }
    cuSel(paramsSelect);

    //сохранить новый индекс
    $(parentDiv).attr("data-innercount", newIndex);
}

function DisplayShowButton(e) {
    var showButton = document.getElementById("search-show-button");
    if (showButton != null && showButton.style.display == "none") {
        showButton.style.display = 'inline-block';
    }
    else {
        console.log("ERROR search-show-button");
    }
    var subscribeButton = document.getElementById("subscribe");
    if (subscribeButton != null && subscribeButton.style.display == "none") {
        subscribeButton.style.display = 'inline-block';
    }
    else {
        console.log("ERROR subscribe-button-show");
    }
}

function AddSubscriptionByEnter(source, e) {
    if (event.keyCode == 13) {
        return addNewSubscription(source);
    }
    return true;
}

/**
  * добавить метку о том, что поисковый запрос нужно сохранить в качестве подписки
  */
function isNullOrWhitespace(input) {

    if (typeof input === 'undefined' || input == null) return true;

    return input.replace(/\s/g, '').length < 1;
}
function addNewSubscription(source) {

    var parentForm = $(source).parents("form")[0];

    var titleValue = $(parentForm).find("input[name=\"NewSubscriptionTitle\"]").val();
    if (isNullOrWhitespace(titleValue)) {
        confirm("Укажите новый заголовок Подписки");
        return false;
    }
    $(parentForm).find("input[name=\"NewSubscriptionAdd\"]").val(true);

    //document.myform.action = "/search/addsubscription";
    return true;
}
/*
 * «срок трудового договора» поля должны показываться только, если выбираешь «срочный»
 */
function toggleContractTime(e) {
    var parent = $('.contract-time-period');

    if (parent.length === 0) {
        return;
    }

    var currentValue = "";
    if (e != undefined) {
        currentValue = e.target.innerText;
    } else {
        currentValue = $('#cusel-scroll-' + 'ContractTypeValue').find('span.cuselActive')[0].textContent;
    }
    if (currentValue.indexOf("ессроч") > -1) {
        $(parent).fadeOut("slow");
        $('#ContractTimeYears').val(null);
        $('#ContractTimeMonths').val(null);
    } else {
        $(parent).fadeIn("slow");
    }
}