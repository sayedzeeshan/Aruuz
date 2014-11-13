var meter = "";
var poet = "";
var type = "";
var meter_taqti = "";
var editorID = 0;
var sectionID = "";
var inputID = -1;
var hrefOneState = 0;
var hrefTwoState = 1;

$(document).ready(function () {
   
    $('textarea').UrduEditor();
    $('textarea').attr('style', 'width:100%');
    $(".alert").alert();
    $('.carousel').carousel();
    $('[data-toggle="popover"]').popover({
        trigger: 'hover',
        'placement': 'top auto',
        'container': 'body',
        'delay': { show: 0, hide: 0 },
    });
    $('[data-toggle="tooltip"]').popover({
        trigger: 'hover',
        'placement': 'top auto',
        'container': 'body',
        'delay': { show: 0, hide: 0 },
    });
    $("#mySearchForm").on("submit", function (e) {
        var searchTerm = $('.search').val();
        if (searchTerm.length > 0) {
            var url1 = "/examples/search/?searchstring=" + searchTerm;
            pathArray = window.location.href.split('/');
            protocol = pathArray[0];
            host = pathArray[2];
            url = protocol + '//' + host + url1;
            window.location.href = url1;
            return false;
        }
        return false;
    });
});
$(window).load(function () {
    //$(".check").removeClass("hidden");
    var h = document.getElementsByTagName("body")[0];
    if ($("#poetrypanel").is(":visible")) {
        var $span = $(".check2");
        var max = 0;
        var width = 0;
        for (var i = 0; i < $span.length; i++) {
            var s = document.createElement("span");
            s.innerHTML = $span[i].innerHTML;
            s.setAttribute("class", "urdu-large");
            h.appendChild(s);
            width = s.offsetWidth; //width for the default font
            h.removeChild(s);

            if (max < width) {
                max = width;
            }
        }
        var len = $("#poetrypanel").outerWidth();
        var percentage = ((max + 30) / len) * 100;
        // $(".check").addClass("hidden");
        if (percentage < 97) {
            $("#poetrytable").addClass("justifyme");
            // $(".table").addClass("table-bordered");
            $(".data").css("text-align", "justify");
            $(".data").css("text-align-last", "justify");
            $(".data").css("-moz-text-align-last", "justify");
            $(".data").css("-ms-text-align-last", "justify");
            $(".data").css("height", "2em");
            $(".data").css("line-height", "2");
            $(".data").css("content", "");
            $(".data").css("display", "inline-block");
            $(".data").css("width", percentage + "%");

        }
    }
    else
    {
        var $span = $(".check");
        var max = 0;
        var width = 0;
        for (var i = 0; i < $span.length; i++) {
            var s = document.createElement("span");
            s.innerHTML = $span[i].innerHTML;
            s.setAttribute("class", "urdu-medium");
            h.appendChild(s);
            width = s.offsetWidth; //width for the default font
            h.removeChild(s);

            if (max < width) {
                max = width;
            }
        }
        var len = $("#poetrypanel2").outerWidth();
        var percentage = ((max + 40) / len) * 100;
        // $(".check").addClass("hidden");
        if (percentage < 97) {
            $("#poetrytable2").addClass("justifyme");
            // $(".table").addClass("table-bordered");
            $(".data2").css("text-align", "justify");
            $(".data2").css("text-align-last", "justify");
            $(".data2").css("-moz-text-align-last", "justify");
            $(".data2").css("-ms-text-align-last", "justify");
            $(".data2").css("height", "2em");
            $(".data2").css("line-height", "2");
            $(".data2").css("content", "");
            $(".data2").css("display", "inline-block");
            $(".data2").css("width", percentage + "%");

        }
    }

});
function mozun() {
    $("[id^=namozun_]").addClass('hidden');
    $("#toplabel").text("میری شاعری (موزوں)");
}
function toggleHref(e)
{
    if (e == '1') {
        $('html, body').animate({
            scrollTop: $("#contOne").offset().top
        }, 700);

        if (hrefOneState == 0) {
            if (hrefTwoState == 1) { 
                $('#hrefOne').removeClass('glyphicon-minus-sign');
                $('#hrefOne').addClass('glyphicon-plus-sign');
                hrefOneState = 1;
            }
            else
            {
                $('#hrefOne').removeClass('glyphicon-minus-sign');
                $('#hrefOne').addClass('glyphicon-plus-sign');

                hrefOneState = 1;
            }
        }
        else
        {
            if (hrefTwoState == 1) { 
                $('#hrefOne').removeClass('glyphicon-plus-sign');
                $('#hrefOne').addClass('glyphicon-minus-sign');

                hrefOneState = 0;
            }
            else { //toggle
                $('#hrefOne').removeClass('glyphicon-plus-sign');
                $('#hrefOne').addClass('glyphicon-minus-sign');
                $('#hrefTwo').removeClass('glyphicon-minus-sign');
                $('#hrefTwo').addClass('glyphicon-plus-sign');
                hrefOneState = 0;
                hrefTwoState = 1;
            }
        }
    }
    else if (e == '2') {
        $('html, body').animate({
            scrollTop: $("#contOne").offset().top
        }, 700);

        if (hrefTwoState == 0) {
            if (hrefOneState == 1) {
                $('#hrefTwo').removeClass('glyphicon-minus-sign');
                $('#hrefTwo').addClass('glyphicon-plus-sign');

                hrefTwoState = 1;
            }
            else {
                $('#hrefTwo').removeClass('glyphicon-minus-sign');
                $('#hrefTwo').addClass('glyphicon-plus-sign');

                hrefTwoState = 1;
            }

        }
        else {
            if (hrefOneState == 1) {
                $('#hrefTwo').removeClass('glyphicon-plus-sign');
                $('#hrefTwo').addClass('glyphicon-minus-sign');

                hrefTwoState = 0;
            }
            else {
                //toggle
                $('#hrefOne').removeClass('glyphicon-minus-sign');
                $('#hrefOne').addClass('glyphicon-plus-sign');
                $('#hrefTwo').removeClass('glyphicon-plus-sign');
                $('#hrefTwo').addClass('glyphicon-minus-sign');

                hrefOneState = 1;
                hrefTwoState = 0;
            }

        }

    }
}
function taqtiPoetry(e) {
    $('html, body').animate({
        scrollTop: $("#contOne").offset().top
    }, 700);

    if (hrefTwoState == 0) {
        if (hrefOneState == 1) {
            $('#hrefTwo').removeClass('glyphicon-minus-sign');
            $('#hrefTwo').addClass('glyphicon-plus-sign');
            hrefTwoState = 1;
        }
        else
        {
            $('#hrefTwo').removeClass('glyphicon-minus-sign');
            $('#hrefTwo').addClass('glyphicon-plus-sign');
            hrefTwoState = 1;
        }
       
    }
    else {
        if (hrefOneState == 1) {
            $('#hrefTwo').removeClass('glyphicon-plus-sign');
            $('#hrefTwo').addClass('glyphicon-minus-sign');
            hrefTwoState = 0;
        }
        else {
            //toggle
            $('#hrefOne').removeClass('glyphicon-minus-sign');
            $('#hrefOne').addClass('glyphicon-plus-sign');
            $('#hrefTwo').removeClass('glyphicon-plus-sign');
            $('#hrefTwo').addClass('glyphicon-minus-sign');
            hrefOneState = 1;
            hrefTwoState = 0;
        }
       
    }
    if (document.getElementById("taqtiDiv") !== null) {
    var url = "/taqti/poetry2";
    var values = {
        "id": e - 65536
    };
    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken(values),
        async: true,
        timeout: 60000,
        success: function (data) {
            $('#taqtiDiv').replaceWith(data);
        },
        error: function (x, t, m) {
            if (t == 'timeout') {
                var data = "<div class='panel panel-body'><span class='label label-default urdu-large'>ٹائم آوٹ خرابی: کافی دیر گذرنے کے بعد بھی آپ کا نتیجہ برامد نہیں ہوا اس کے لئے ہم معذرت خواہ ہیں۔ آپ چاہیں تو یہ خرابی رپورٹ کر سکتے ہیں۔</span></div>";
                $('#taqtiDiv').replaceWith(data);
            }
        }
    });
}
}
function taqtiMyPoetry(e) {
    $('html, body').animate({
        scrollTop: $("#contOne").offset().top
    }, 700);

    if (hrefTwoState == 0) {
        if (hrefOneState == 1) {
            $('#hrefTwo').removeClass('glyphicon-minus-sign');
            $('#hrefTwo').addClass('glyphicon-plus-sign');
            hrefTwoState = 1;
        }
        else {
            $('#hrefTwo').removeClass('glyphicon-minus-sign');
            $('#hrefTwo').addClass('glyphicon-plus-sign');
            hrefTwoState = 1;
        }

    }
    else {
        if (hrefOneState == 1) {
            $('#hrefTwo').removeClass('glyphicon-plus-sign');
            $('#hrefTwo').addClass('glyphicon-minus-sign');
            hrefTwoState = 0;
        }
        else {
            //toggle
            $('#hrefOne').removeClass('glyphicon-minus-sign');
            $('#hrefOne').addClass('glyphicon-plus-sign');
            $('#hrefTwo').removeClass('glyphicon-plus-sign');
            $('#hrefTwo').addClass('glyphicon-minus-sign');
            hrefOneState = 1;
            hrefTwoState = 0;
        }

    }
    if (document.getElementById("taqtiDiv") !== null) {
        var url = "/taqti/mypoetry2";
        var values = {
            "id": e
        };
        $.ajax({
            type: 'POST',
            url: url,
            data: AddAntiForgeryToken(values),
            async: true,
            timeout: 60000,
            success: function (data) {
                $('#taqtiDiv').replaceWith(data);
            },
            error: function (x, t, m) {
                if (t == 'timeout') {
                    var data = "<div class='panel panel-body'><span class='label label-default urdu-large'>ٹائم آوٹ خرابی: کافی دیر گذرنے کے بعد بھی آپ کا نتیجہ برامد نہیں ہوا اس کے لئے ہم معذرت خواہ ہیں۔ آپ چاہیں تو یہ خرابی رپورٹ کر سکتے ہیں۔</span></div>";
                    $('#taqtiDiv').replaceWith(data);
                }
            }
        });
    }
}
function outputTaqti() {
    var url = "/taqti/output2";
    var values = {
        "id": $('#inputID').text()
    };
    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken(values),
        async: true,
        timeout: 60000,
        success: function (data) {
            $('#detailsDiv').replaceWith(data);
        },
        error: function (x, t, m) {
            if (t == 'timeout') {
                var data = "<div class='panel panel-body'><span class='label label-default urdu-large'>ٹائم آوٹ خرابی: کافی دیر گذرنے کے بعد بھی آپ کا نتیجہ برامد نہیں ہوا اس کے لئے ہم معذرت خواہ ہیں۔ آپ چاہیں تو یہ خرابی رپورٹ کر سکتے ہیں۔</span></div>";
                $('#detailsDiv').replaceWith(data);
            }
        }
    });
}
function outputCreate() {
    var url2 = "/create/output2";
    var values2 = {
        "id": $('#inputID2').text()
    };
    $.ajax({
        type: 'POST',
        url: url2,
        data: AddAntiForgeryToken(values2),
        async: true,
        timeout: 60000,
        success: function (data) {
            $('#detailsDiv2').replaceWith(data);
        },
        error: function (x, t, m) {
            if (t == 'timeout') {
                var data = "<div class='panel panel-body'><span class='label label-default urdu-large'>ٹائم آوٹ خرابی: کافی دیر گذرنے کے بعد بھی آپ کا نتیجہ برامد نہیں ہوا اس کے لئے ہم معذرت خواہ ہیں۔ آپ چاہیں تو یہ خرابی رپورٹ کر سکتے ہیں۔</span></div>";
                $('#detailsDiv').replaceWith(data);
            }
        }
    });
}
function resultTaqti() {
    var url = "/taqti/result2";
    var values = {
        "text": $('#inputTextRes').text(),
        "isChecked": $('#inputCheckRes').text()
    };
    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken(values),
        async: true,
        timeout: 60000,
        success: function (data) {
            $('#detailsDiv').replaceWith(data);
        },
        error: function (x, t, m) {
            if (t == 'timeout') {
                var data = "<div class='panel panel-body'><span class='label label-default urdu-large'>ٹائم آوٹ خرابی: کافی دیر گذرنے کے بعد بھی آپ کا نتیجہ برامد نہیں ہوا اس کے لئے ہم معذرت خواہ ہیں۔ آپ چاہیں تو یہ خرابی رپورٹ کر سکتے ہیں۔</span></div>";
                $('#detailsDiv').replaceWith(data);
            }
        }
    });

}
function resultCreate() {
    var url2 = "/create/result2";
    var values2 = {
        "text": $('#inputTextRes2').text()
    };
    $.ajax({
        type: 'POST',
        url: url2,
        data: AddAntiForgeryToken(values2),
        async: true,
        timeout: 60000,
        success: function (data) {
            $('#detailsDiv2').replaceWith(data);
        },
        error: function (x, t, m) {
            if (t == 'timeout') {
                var data = "<div class='panel panel-body'><span class='label label-default urdu-large'>ٹائم آوٹ خرابی: کافی دیر گذرنے کے بعد بھی آپ کا نتیجہ برامد نہیں ہوا اس کے لئے ہم معذرت خواہ ہیں۔ آپ چاہیں تو یہ خرابی رپورٹ کر سکتے ہیں۔</span></div>";
                $('#detailsDiv').replaceWith(data);
            }
        }
    });

}
function publish() {
    var data = "<div class='modal-body urdu'>\
        <label for='poet-name' class='urdu-naskh-medium'>\
        *شاعر کا نام\
        </label>\
        <input type='text' class = 'form-control data input-lg urdu-medium' placeholder = 'آپ کا نام' maxlength='20' id='poet-name'>\
        <br>\
        <label for='poet-url' class='urdu-naskh-medium'>\
    فیسبک یا ویب سائٹ (شامل کرنا ضروری نہیں) \
    </label>\
<input class = 'form-control data' placeholder = 'مثال: http://www.facebook.com/xyz'  id='poet-url'>\
    <br>\
    <label for='title' class='urdu-naskh-medium'>\
    *عنوان \
    </label>\
    <input class = 'form-control data input-lg urdu-medium' value = ''  id='title' maxlength='100'>\
    </div>";
    $("#myModal div.modal-body").replaceWith(data);
    $("#myModal h4.modal-title").replaceWith("<h4 class='modal-title urdu-naskh' id='modalLabel'>" + "کلام شائع کریں یا لنک حاصل کریں" + "</div>");
    $("#button-label").replaceWith("<p id='button-label' class='urdu-naskh'>شائع کریں/لنک حاصل کریں</p>");
    $("#myModal").removeClass("opened");
    $("#button-label").removeClass("close-it");
    $('#poet-name').UrduEditor(); 
    $('#title').UrduEditor();
    $("#button-label").addClass("publish");
    $('#myModal').modal('toggle');
}
function like(e) {
    var url = "/taqti/like";
    $("#likebutton").addClass("disabled");
    $("#dislikebutton").addClass("disabled");
    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken({ url: e }),
        async: false,
        success: function (d) {          
         
        }
    });
   
}
function dislike(e) {
    var url = "/taqti/dislike";
    $("#likebutton").addClass("disabled");
    $("#dislikebutton").addClass("disabled");
    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken({ url: e }),
        async: false,
        success: function (d) {
            $("#likebutton").addClass("disabled");
        }
    });
}
function populatepoetry() {
    var url = "/examples/metersList";
    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken({ url: e }),
        async: false,
        success: function (d) {
            $("#poetryDiv").replace(d);
        }
    });
}
function taqtiClicked() {
    var changed = $(".data").each(function () { $(this).attr("id"); });

    if (changed.length == 0) {
        $(".alert").show();
        $(".alert").delay(5000).slideUp();
    }
    else {
        var text = "";
        for (i = 0; i < changed.length; i++) {
            text = $('input').filter("[id^=" + $(changed[i]).attr('id') + "]").val();
            var value = $(changed[i]).val();
            var mat = $(changed[i]).attr('id').match("row0_(.*)_Score");
            var id = mat[1].replace(/[^0-9]/g, '');
            var met = mat[1].replace(/[0-9-]/g, '').replace(/[_]/g, ' ');
            var values = {
                "text": text,
                "meter": met,
                "id" : id
            }
            var idText =$(changed[i]).attr('id');
            sectionID = idText.substring(12, idText.length);
            var url = "/create/partialindex";
            $('input').filter("[id^=" + $(changed[i]).attr('id') + "]").focus();
            $('input').filter("[id^=" + $(changed[i]).attr('id') + "]").after("<img src='/icons/ajax-loader.gif'></img>").delay(500);

            $.ajax({type: 'POST',
                url: url,
                data: AddAntiForgeryToken(values),
                async: false,
                success: function (data) {
                $('tbody').filter("[id^=section0_" + sectionID + "]").addClass('remove');
            $('tbody').filter("[id^=section1_" + sectionID + "]").addClass('remove');
            $('tbody').filter("[id^=section2_" + sectionID + "]").addClass('remove');
            $('tbody').filter("[id^=section2_" + sectionID + "]").after(data);
            $('.remove').remove();
            $('tbody').filter("[id^=section0_" + sectionID + "]").focus();
        }
    });
        }
    }

}
function dictionaryClicked() {
    var data = "<div class='modal-body'><label for='word-input' class='urdu-naskh-medium'>لفظ</label> <input class = 'form-control data urdu-medium input-lg' value = ''  id='word-input'></input></div>";
    $("#myModal div.modal-body").replaceWith(data);
    $("#myModal h4.modal-title").replaceWith("<h4 class='modal-title urdu-naskh' id='modalLabel'>" + "مفرد الفاظ کی تقطیع" + "</div>");
    $("#button-label").replaceWith("<p id='button-label' class='urdu-naskh'>تقطیع</p>");
    $("#myModal").removeClass("opened");
    $("#button-label").removeClass("close-it");
    $('#word-input').UrduEditor();
    $("#button-label").addClass("dictionary");
    $('#myModal').modal('toggle');
}
function reportClicked(e, name) {
    var data = "<div class='modal-body urdu-medium'><label for='name-input' class='urdu-naskh-medium'>نام</label><input class = 'form-control data' id='name-input'></input><label for='email-input' class='urdu-naskh-medium'>ای-میل</label><input class = 'form-control data' id='email-input'></input> <label for='comments-input' class='urdu-naskh-medium'>تبصرہ </label><textarea class = 'form-control data' id='comments-input'></textarea></div>";

    if (name != null)
    {
        data = "<div class='modal-body urdu-medium'><label for='name-input'>نام</label><input class = 'form-control data' id='name-input' value ='" + name + "'></input> <label for='comments-input'>تبصرہ </label><textarea class = 'form-control data' id='comments-input'></textarea></div>";
       
    }
    $("#myModal div.modal-body").replaceWith(data);
    $("#myModal h4.modal-title").replaceWith("<h4 class='modal-title urdu-naskh' id='modalLabel'>" + "خرابی رپورٹ کریں" + "</div>");
    $("#button-label").replaceWith("<p id='button-label' class='urdu-naskh'>رپورٹ</p>");
    $("#myModal").removeClass("opened");
    $("#button-label").removeClass("close-it");
    $('#name-input').UrduEditor();
    $('#name-input').addClass("urdu-medium");
    $('#comments-input').UrduEditor();
    $('#name-input').addClass("urdu");
    $("#button-label").addClass("report");
    if (name != null) {
        $("#button-label").removeClass("report");
        $("#button-label").addClass("report2");
    }


    inputID = e;

    $('#myModal').modal('toggle');
}
function infoClicked(e) {
    $("#myModal").removeClass("opened");
    $("#button-label").removeClass("close-it");
    $("#button-label").addClass("info");
    var url = "/taqti/info";
    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken({id: e}),
        async: false,
        success: function (d) {

            var data = "<div class='modal-content urdu'>" + d + "</div>";
            $("#myModal div.modal-content").replaceWith(data);
        }
    });

    $('#myModal').modal('toggle');
}
function modalButtonClicked() {
    if ($("#myModal").hasClass("opened")) {
        if ($("#button-label").hasClass("close-it")) {
            $('#myModal').modal('toggle');
            $("#myModal").removeClass("opened");
            $("#button-label").removeClass("close-it");
        }
    }
    else {

        if ($("#button-label").hasClass("dictionary")) {
            var values = {
                "text": $("#word-input").val(),
                "isChecked": false
            }

            var imgCode = "<div class='modal-body'> <img src='/icons/ajax-loader.gif'></img></div>";
            $("#myModal div.modal-body").replaceWith(imgCode).delay(1000);


            var url = "/create/words";

            $.post(url, AddAntiForgeryToken(values), function (data) {

                var values = "<div class='modal-body'>" + data + "</div>";
                $("#myModal div.modal-body").replaceWith(values);
                $("#button-label").replaceWith("<p id='button-label' class='urdu'>بند کریں</p>");
                $("#button-label").addClass("close-it");
                $("#myModal").addClass("opened");

            });
        }
        else if($("#button-label").hasClass("report"))
        {
            if ($("#name-input").val() == '')
            {
                $("#name-input").after("<div id = 'temp' class = 'red urdu-naskh-medium'><p>نام لکھنا ضروری ہے </p></div>");
                setTimeout(function () {
                    $('.red').remove();
                }, 2000);
                $("#name-input").focus();
            }
            else if ($("#email-input").val() == '')
            {
                $("#email-input").after("<div id = 'temp' class = 'red urdu-naskh-medium'><p>ای-میل لکھنا ضروری ہے </p></div>").delay(2000);
                setTimeout(function () {
                    $('.red').remove();
                }, 2000);
                $("#email-input").focus();
            }
            else if ($("#comments-input").val() == '') {
                $("#comments-input").after("<div id = 'temp' class = 'red urdu-naskh-medium'><p>تبصرہ لکھنا ضروری ہے </p></div>").delay(2000);
                setTimeout(function () {
                    $('.red').remove();
                }, 2000);
                $("#comments-input").focus();
            }
            else if (!IsEmail($("#email-input").val())) {
                $("#email-input").after("<div id = 'temp' class = 'urdu-naskh-medium red '><p>ای-میل غلط لکھا ہے </p></div>").delay(2000);
                setTimeout(function () {
                    $('.red').remove();
                }, 2000);
                $("#email-input").focus();
            }
            else {

                var values = {
                    "inputid": inputID,
                    "name": $("#name-input").val(),
                    "email": $("#email-input").val(),
                    "comments": $("#comments-input").val()
                }

                var imgCode = "<div class='modal-body'> <img src='/icons/ajax-loader.gif'></img> </div>";
                $("#myModal div.modal-body").replaceWith(imgCode).delay(300);


                var url = "/taqti/report";

                $.post(url, AddAntiForgeryToken(values), function (data) {
                    var values = "<div class='modal-body'>" + "شکریہ۔ آپ کا مسئلہ رپورٹ ہو گیا ہے۔ امید ہے اس کا جلد سدِ باب کیا جائے گا۔" + "</div>";
                    $("#myModal div.modal-body").replaceWith(values);
                    $("#button-label").replaceWith("<p id='button-label' class='urdu'>بند کریں</p>");
                    $("#button-label").addClass("close-it");
                    $("#myModal").addClass("opened");

                });
            }
        }
        else if ($("#button-label").hasClass("report2")) {
           if ($("#comments-input").val() == '') {
                $("#comments-input").after("<div id = 'temp' class = 'urdu-naskh-medium red'><p>تبصرہ لکھنا ضروری ہے </p></div>").delay(2000);
                setTimeout(function () {
                    $('.red').remove();
                }, 2000);
                $("#comments-input").focus();
            }
            else {

                var values = {
                    "inputid": inputID,
                    "name": $("#name-input").val(),
                    "email": $("#email-input").val(),
                    "comments": $("#comments-input").val()
                }

                var imgCode = "<div class='modal-body'> <img src='/icons/ajax-loader.gif'></img> </div>";
                $("#myModal div.modal-body").replaceWith(imgCode).delay(300);


                var url = "/taqti/report";

                $.post(url, AddAntiForgeryToken(values), function (data) {
                    var values = "<div class='modal-body'>" + "شکریہ۔ آپ کا مسئلہ رپورٹ ہو گیا ہے۔ امید ہے اس کا جلد سدِ باب کیا جائے گا۔" + "</div>";
                    $("#myModal div.modal-body").replaceWith(values);
                    $("#button-label").replaceWith("<p id='button-label' class='urdu'>بند کریں</p>");
                    $("#button-label").addClass("close-it");
                    $("#myModal").addClass("opened");

                });
            }
        }
        else if ($("#button-label").hasClass("publish"))
        {
            if ($("#poet-name").val() == '') {
                $("#poet-name").after("<div id = 'temp' class = 'red urdu-naskh-medium'><p>نام لکھنا ضروری ہے </p></div>");
                setTimeout(function () {
                    $('.red').remove();
                }, 2000);
                $("#poet-name").focus();
            }
            else if ($("#title").val() == '') {
                $("#title").after("<div id = 'temp' class = 'urdu-naskh-medium red'><p>عنوان لکھنا ضروری ہے </p></div>").delay(2000);
                setTimeout(function () {
                    $('.red').remove();
                }, 2000);
                $("#title").focus();
            }
            else {

                var values = {
                    "title": $("#title").val(),
                    "name": $("#poet-name").val(),
                    "url": $("#poet-url").val(),
                    "text": $("#inputTextRes").text(),
                    "percentage": $("#percentage").text()
                }

                var imgCode = "<div class='modal-body'> <img src='/icons/ajax-loader.gif'></img> </div>";
                $("#myModal div.modal-body").replaceWith(imgCode).delay(300);

                var url = "/mypoetry/publish";
                $.post(url, AddAntiForgeryToken(values), function (data) {
                    $("#myModal div.modal-body").replaceWith(data);
                    $("#button-label").replaceWith("<p id='button-label' class='urdu'>بند کریں</p>");
                    $("#button-label").addClass("close-it");
                    $("#myModal").addClass("opened");
                    $("#linkbutton").addClass("disabled");
                    $("#linkbutton2").addClass("disabled");


                });
            }
        }
    }
}
function wordClicked(word) {

  
    var data = "<div class='modal-body'></div>";
    $("#myModal div.modal-body").replaceWith(data);
    $("#myModal h4.modal-title").replaceWith("<h4 class='modal-title urdu-naskh' id='modalLabel'>" + "لفظ کی تقطیع" + "</div>");
    $("#myModal").removeClass("opened");
    $("#button-label").replaceWith("<p id='button-label' class='urdu-naskh'>بند کریں</p>");
    $("#button-label").removeClass("close-it");
    $('#myModal').modal('toggle');


    if (word == '---') {
        var values = "<div class='modal-body urdu-naskh-medium'>" + "اس لفظ کی تقطیع کے بارے میں ہمیں تحفظات ہیں۔ اگر تو اس لفظ میں املا کی اغلاط ہیں تو ان کو درست لکھیں، یا اگر  یہ لفظ کئی الفاظ کا مجموعہ ہے تو سب الفاظ کو الگ الگ لکھیں۔  " + "</div>";
        $("#myModal div.modal-body").replaceWith(values);
        $("#button-label").addClass("close-it");
        $("#myModal").addClass("opened");
    }
    else {
        var values = {
            "text": word,
            "isChecked": false
        }

        var imgCode = "<div class='modal-body'> <img src='/icons/ajax-loader.gif'></img></div>";
        $("#myModal div.modal-body").replaceWith(imgCode).delay(1000);


        var url = "/create/words";

        $.post(url, AddAntiForgeryToken(values), function (data) {

            var values = "<div class='modal-body'>" + data + "</div>";
            $("#myModal div.modal-body").replaceWith(values);
            $("#button-label").addClass("close-it");
            $("#myModal").addClass("opened");
        });
    }

       
}
function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}
function IsUrl(url)
{
    if(/^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/.test(url))
    {
        return true;
    }
    else
    {
        return false;
    }
}
function rowClicked(id) {

    var a = "#" + id;
    var OriginalContent = $(a).text().replace(/\s+/g, " ");
    OriginalContent = OriginalContent.substring(7, OriginalContent.length);
    var d = "<tr><td colspan = '8'><input class = 'form-control data urdu-small input-lg' id='Editor_" + id +"' value = '" + OriginalContent + "' ></input> </td></tr>";
    $('#saveicon').removeClass('glyphicon-floppy-saved');
    $('#saveicon').addClass('glyphicon-floppy-save');
    if (!$(a).hasClass('done')) {
        $(a).addClass('done');
        $(a).addClass('success');
        $(a).after(d);
        var ed = "[id^=Editor_" + id + "]";
        $('input').filter(ed).UrduEditor();
        //$('input').filter(ed).css("font-size","small");
        setTimeout(function () { $('input').filter(ed).focus() }, 300);
    }
}
function radioLoaded() {
    $('option2').button('toggle');
}
function validate() {
    if (document.getElementById('option1').checked) {
        $('option2').button('toggle');
        if (meter.length == 0) {
            $('[id^=section2]').show();
            $("#outputTable").addClass("table-bordered");
        }
        else {
            var abs = '[id*=-' + meter + '-]';
            $('[id^=section2]').filter(abs).show();
            $("#outputTable").addClass("table-bordered");
        }
    }
    else {
        if (meter.length == 0) {
            $('[id^=section2]').hide();
            $("#outputTable").removeClass("table-bordered");
        }
        else {
            var abs = '[id*=-' + meter + '-]';
            $('[id^=section2]').filter(abs).hide();
            $("#outputTable").removeClass("table-bordered");
        }
    }
}
function menuInit(e) {
    meter = e.substring(15, e.length);
    /*.split('|')[0];
    meter = $.trim(meter);
    meter = meter.replace(/\s/g, '_').replace("/", "");*/
    var select = "[id^=" + e.split('|')[0] + "]";
    $('[id^=meter_taqti_li_]').removeClass('active');
    $(select).addClass('active');
    $('[id^=section0]').hide();
    $('[id^=section1]').hide();
    $('[id^=section2]').hide();
    values = meter.split('|');

    for (var i = 0; i < values.length; i++) {

        var abs = '[id*=-' + values[i].replace("/", "").replace("(", "").replace(")", "") + '-]';
        $("tbody").filter(abs).show();
    }

    if (document.getElementById('option2').checked) {
        $("#outputTable").removeClass("table-bordered");
        $('[id^=section2]').hide();
    }
}
function menuSelect(e) {
    var select = "[id^=meter_taqti_li_" + e.split('|')[0] + "]";
    $('[id^=meter_taqti_li_]').removeClass('active');
    $(select).addClass('active');
    if (e == "all") {
        meter = "all";
        $('[id^=section0]').show();
        $('[id^=section1]').show();
        if (document.getElementById('option1').checked) {
            $('[id^=section2]').show();
            $("#outputTable").addClass("table-bordered");

        }
        else {
            $('[id^=section2]').hide();
            $("#outputTable").removeClass("table-bordered");

        }
    }
    else {
        meter = e;
        $('[id^=section0]').hide();
        $('[id^=section1]').hide();
        $('[id^=section2]').hide();
        values = e.split('|');

        for (var i = 0; i < values.length; i++) {

            var abs = '[id*=-' + values[i].replace("/", "").replace("(", "").replace(")", "") + '-]';
            $("tbody").filter(abs).show();
        }

        $("#outputTable").addClass("table-bordered");


        if (document.getElementById('option2').checked) {
            $("#outputTable").removeClass("table-bordered");
            $('[id^=section2]').hide();
        }
    }
}
function menuSelectExamples(e) {
    var d = e.replace("/", "");
    var select = "[id*=-" + d + "-]";
    $('[id^=meter_li_]').removeClass('active');
    $(select).addClass('active');
    $(this).addClass('active');
    if (e == "all") {
        meter = "";
        if (poet == "") {
            if (type == "") {
                $('tr').show();
            }
            else {
                var tn = '[id*=' + type + ']';
                $("tr").filter(tn).show();
            }
        }
        else {
            if (type == "") {
                var tn = '[id*=-' + poet + '-]';
                $("tr").filter(tn).show();
            }
            else {
                var tn = '[id*=' + type + ']';
                var tn2 = '[id*=' + poet + ']';
                $("tr").filter(tn).filter(tn2).show();
            }

        }
    }
    else {
        meter = e;

        $('tr').hide();
        if (poet == "") {
            if (type == "") {
                var abs = '[id*=-' + e + '-]';
                $("tr").filter(abs).show();
            }
            else {
                var tn = '[id*=' + type + ']';
                var tn2 = '[id*=-' + e + '-]';
                $("tr").filter(tn).filter(tn2).show();
            }
        }
        else {
            if (type == "") {
                var tn = '[id*=' + poet + ']';
                var tn2 = '[id*=-' + e + '-]';
                $("tr").filter(tn).filter(tn2).show();
            }
            else {
                var abs = '[id*=' + type + ']';
                var tn = '[id*=-' + meter + '-]';
                var tn2 = '[id*=' + poet + ']';
                $("tr").filter(abs).filter(tn).filter(tn2).show();
            }
        }
    }
    $('#main').show();
}
function menuSelectPoets(e) {
    var select = "#poet_li_" + e;
    $('[id^=poet_li_]').removeClass('active');
    $(select).addClass('active');
    $(this).addClass('active');
    if (e == "all") {
        poet = "";
        if (meter == "") {
            if (type == "") {
                $('tr').show();
            }
            else {
                var tn = '[id*=' + type + ']';
                $("tr").filter(tn).show();
            }
        }
        else {
            if (type == "") {
                var tn = '[id*=-' + meter + '-]';
                $("tr").filter(tn).show();
            }
            else {
                var tn = '[id*=' + type + ']';
                var tn2 = '[id*=-' + meter + '-]';
                $("tr").filter(tn).filter(tn2).show();
            }

        }
    }
    else {
        poet = e;

        $('tr').hide();
        if (meter == "") {
            if (type == "") {
                var abs = '[id*=' + e + ']';
                $("tr").filter(abs).show();
            }
            else {
                var tn = '[id*=' + type + ']';
                var tn2 = '[id*=' + e + ']';
                $("tr").filter(tn).filter(tn2).show();
            }
        }
        else {
            if (type == "") {
                var tn = '[id*=-' + meter + '-]';
                var tn2 = '[id*=' + e + ']';
                $("tr").filter(tn).filter(tn2).show();
            }
            else {
                var abs = '[id*=' + type + ']';
                var tn = '[id*=-' + meter + '-]';
                var tn2 = '[id*=' + poet + ']';
                $("tr").filter(abs).filter(tn).filter(tn2).show();
            }
        }
    }
    $('#main').show();

}
function menuSelectTypes(e) {
    var select = "#type_li_" + e;
    $('[id^=type_li_]').removeClass('active');
    $(select).addClass('active');
    $(this).addClass('active');
    if (e == "all") {
        type = "";
        if (meter == "") {
            if (poet == "") {
                $('tr').show();
            }
            else
            {
                var tn = '[id*=' + poet + ']';
                $("tr").filter(tn).show();
            }
        }
        else {
            if (poet == "") {
                var tn = '[id*=-' + meter + '-]';
                $("tr").filter(tn).show();
            }
            else {
                var tn = '[id*=' + poet + ']';
                var tn2 = '[id*=-' + meter + '-]';
                $("tr").filter(tn).filter(tn2).show();
            }
           
        }
    }
    else {
        type = e;

        $('tr').hide();
        if (meter == "") {
                if (poet == "") {
                var abs = '[id*=' + e + ']';
                $("tr").filter(abs).show();
            }
            else {
                    var tn = '[id*=' + poet + ']';
                    var tn2 = '[id*=' + e + ']';
                    $("tr").filter(tn).filter(tn2).show();
            }
        }
        else {
            if (poet == "") {
                var tn = '[id*=-' + meter + '-]';
                var tn2 = '[id*=' + e + ']';
                $("tr").filter(tn).filter(tn2).show();
            }
            else {
                var abs = '[id*=' + type + ']';
                var tn = '[id*=-' + meter + '-]';
                var tn2 = '[id*=' + poet + ']';
                $("tr").filter(abs).filter(tn).filter(tn2).show();
            }
        }
    }
    $('#main').show();
}
function textCopy() {
    $("#myModal").addClass("opened");
    $("#button-label").addClass("close-it");
    $("#button-label").addClass("info");
    
    var a = "td.data";
    var OriginalContent = $(a);
    var text = "";

    for (var a = 0; a < OriginalContent.length; a++)
    {
        text += $(OriginalContent[a]).text().replace(/\s+/g, " ") + "<br>";
    }
    var values = "<div class='modal-body'>" + text + "</div>";
    $("#myModal div.modal-body").replaceWith(values);


    $('#myModal').modal('toggle');
}
function saveClicked() {
    if ($('#saveicon').hasClass('glyphicon-floppy-save')) {
        var a = "[id^=row0]";
        var OriginalContent = $(a).text().replace(/\s+/g, " ").replace(/تدوین/g, "\n").replace(/\n/,"");
        var values = {
            "text": OriginalContent,
        };
        var url = "/create/save";

        $.ajax({
            type: 'POST',
            url: url,
            data: AddAntiForgeryToken(values),
            async: false,
            success: function (data) {
                $('#saveicon').removeClass('glyphicon-floppy-save');
                $('#saveicon').addClass('glyphicon-floppy-saved');
            }
        });
    }

}
function search()
{
    var searchTerm = $('.search').val();
    if (searchTerm.length > 1) {
        var url1 = "/examples/search/?searchstring=" + searchTerm;
        pathArray = window.location.href.split('/');
        protocol = pathArray[0];
        host = pathArray[2];
        url = protocol + '//' + host + url1;
        window.location.assign(ur1 ); 
        return false;
    }
    return false;

}
function rowTaqti(id, met, isChecked) {
    var a = "#" + id;
    var text = $(a).text().replace(/\s+/g, " ");
    var values = {
        "text": text,
        "meter": met,
        "id": id,
        "isChecked": isChecked
    };
    var url = "/create/partialindexexamples";
    $('tr').filter(a).replaceWith("<tr id = '" + id + "'><td><img src='/icons/ajax-loader.gif'></img></td></tr>").delay(500);

    $.ajax({
        type: 'POST',
        url: url,
        data: AddAntiForgeryToken(values),
        async: false,
        success: function (data) {
            $('tr').filter(a).replaceWith(data);
            setTimeout(function () {
                $('tr').filter(a).removeClass("warning");
                $('tr').filter(a).addClass("active");
            }, 2000);
        },
        error: function () {
            $('tr').filter(a).replaceWith("<tr><td align='center'>" + text +"</td></tr>");
        },
        timeout: 10000
    });
    return false;
}
AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};





