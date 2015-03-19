(function ($) {
    var langArray = [];
    var codes = [];
    var currEdit = null;
    var IsUrdu = 1;
    var EditorId = 0;
    var gOpts;
    var elName;
    var elIndex;
    var SrcElement;
    var Settings = {
        EditorFont: "Urdu Naskh Asiatype",
        EnglishColor: "#CCCCFF",
        UrduColor: "#99FF99"
    };
    codes['a'] = 0x0627;
    codes['b'] = 0x0628;
    codes['c'] = 0x0686;
    codes['d'] = 0x062F;
    codes['e'] = 0x0639;
    codes['f'] = 0x0641;
    codes['g'] = 0x06AF;
    codes['h'] = 0x06BE;
    codes['i'] = 0x06CC;
    codes['j'] = 0x062C;
    codes['k'] = 0x06A9;
    codes['l'] = 0x0644;
    codes['m'] = 0x0645;
    codes['n'] = 0x0646;
    codes['o'] = 0x06C1;
    codes['p'] = 0x067E;
    codes['q'] = 0x0642;
    codes['r'] = 0x0631;
    codes['s'] = 0x0633;
    codes['t'] = 0x062A;
    codes['u'] = 0x0626;
    codes['v'] = 0x0637;
    codes['w'] = 0x0648;
    codes['x'] = 0x0634;
    codes['y'] = 0x06D2;
    codes['z'] = 0x0632;
    codes['A'] = 0x0622;
    codes['B'] = 0xFDFD;
    codes['C'] = 0x062B;
    codes['D'] = 0x0688;
    codes['E'] = 0x0611;
    codes['F'] = 0x0656;
    codes['G'] = 0x063A;
    codes['H'] = 0x062D;
    codes['I'] = 0x0670;
    codes['J'] = 0x0636;
    codes['K'] = 0x062E;
    codes['L'] = 0x0612;
    codes['M'] = 0x0610;
    codes['N'] = 0x06BA;
    codes['O'] = 0x06C3;
    codes['P'] = 0x064F;
    codes['Q'] = 0xFDFA;
    codes['R'] = 0x0691;
    codes['S'] = 0x0635;
    codes['T'] = 0x0679;
    codes['U'] = 0x0621;
    codes['V'] = 0x0638;
    codes['W'] = 0x0624;
    codes['X'] = 0x0698;
    codes['Y'] = 0x06D3;
    codes['Z'] = 0x0630;
    codes['>'] = 0x064E;
    codes['<'] = 0x0650;
    codes[String.fromCharCode(32)] = 32;
    codes[String.fromCharCode(13)] = 13;
    codes[':'] = 0x061B;
    codes[';'] = 0x061B;
    codes[String.fromCharCode(39)] = 0x2018;
    codes[String.fromCharCode(34)] = 0x201C;
    codes[String.fromCharCode(46)] = 0x06D4;
    codes[String.fromCharCode(44)] = 0x060C;
    codes['!'] = 0x0021;
    codes['?'] = 0x061F;
    codes[':'] = 58;
    codes['['] = 0x064C;
    codes[']'] = 0x0652;
    codes['~'] = 0x064B;
    codes['/'] = 0x002F;
    codes['+'] = 0x06E4;
    codes['-'] = 0x002D;
    codes[String.fromCharCode(47)] = 0x00F7;
    codes[String.fromCharCode(37)] = 0x066A;
    codes['('] = 0x0029;
    codes[')'] = 0x0028;
    codes['='] = 0x0653;
    codes['`'] = 0x064D;
    codes['{'] = 0x0657;
    codes['}'] = 0x0651;
    codes['^'] = 0x06C2;
    codes[String.fromCharCode(0x005C)] = 0x060E;
    codes['|'] = 0x0614;
    codes['0'] = 0x30;
    codes['1'] = 0x31;
    codes['2'] = 0x32;
    codes['3'] = 0x33;
    codes['4'] = 0x34;
    codes['5'] = 0x35;
    codes['6'] = 0x36;
    codes['7'] = 0x37;
    codes['8'] = 0x38;
    codes['9'] = 0x39;
    var Diacritics = '[]{}~';
    $.fn.UrduEditor = function () {
        var kbNormal = 1;
        var kbShift = 2;
        var kbAlt = 3;
        var kbCtrl = 4;
        var kbAltGr = 5;
        var bToggleFlag = 0;
        var CurrentKeyboardState = 1;
        //SrcElement = $(this).get(0);
        var size = "14pt";
        return this.each(function () {
            var el = $(this);
            // set the unique identifier            
            $(el).attr("UrduEditorId", getId());
            setAttributes(el, size);
            // keypress handler
            $(el).on('keypress', function (e) {
                var editorId = $(el).attr("UrduEditorId");
                //if (!langArray[editorId]) return;
                e = (e) ? e : (window.event) ? event : null;

                var charCode = (e.charCode) ? e.charCode :
                        ((e.keyCode) ? e.keyCode :
                       ((e.which) ? e.which : 0));
                var whichASC = charCode; // key's ASCII code
                var whichChar = String.fromCharCode(whichASC); // key's character
                /* if ($.browser.msie) {
                     event.keyCode = codes[whichChar];
                 }
                 else if ($.browser.mozilla) {*/
                if ((charCode == 13) || (charCode == 8) || (charCode == 37) || (charCode == 39) || (charCode == 38) || (charCode == 40) || (charCode == 33) || (charCode == 34) || (charCode == 46) || (charCode == 50)) return;
                if (!e.ctrlKey) {
                    if (!find(charCode)) {
                        AddText(String.fromCharCode(codes[whichChar]));
                    }
                    else
                    {
                        AddText(whichChar);
                    }
                   
                    e.preventDefault();
                    e.stopPropagation();
                }
                //}
            });
            $(el).on('keydown', function (e) {
                e = (e) ? e : (window.event) ? event : null;
                if (e) {
                    var charCode = (e.charCode) ? e.charCode : e.keyCode;
                    if (charCode == 17) {
                        CurrentKeyboardState = kbCtrl;
                    }
                    else if (CurrentKeyboardState == kbCtrl) {
                        if (charCode == 32) {
                            //$.fn.UrduEditor.ToggleLanguage();
                            //e.preventDefault();
                            //e.stopPropagation();
                        }
                    }
                }
            });
            $(el).on('keyup', function (e) {
                e = (e) ? e : (window.event) ? event : null;
                if (e) {
                    var charCode = (e.charCode) ? e.charCode : e.keyCode;
                    if (charCode == 17) {
                        CurrentKeyboardState = kbNormal;
                    }
                }
            });
            $(el).on('focus', function (e) {
                //if ($.browser.mozilla) {
                currEdit = e.target;
                //}
                //else if ($.browser.msie)
                //   currEdit = window.event.srcElement;
            });
        });
    };
    function getId() {
        EditorId++;
        return "UrduEditor_" + EditorId;
    }

    function find(e)
    {
        for(var key in codes)
        {
            if(codes[key] == e)
            {
                return true;
            }
        }
        return false;
    }
    function setEditor(e) {
        //if ($.browser.mozilla) {
        currEdit = e.target;
        //}
        //else if ($.browser.msie)
        //    currEdit = window.event.srcElement;
    }
    function setAttributes(el, pt) {
        el.lang = "ur";
        el.dir = "rtl";
        el.wrap = "soft";
        var editorId = $(el).attr("UrduEditorId");
        langArray[editorId] = 1;
        IsUrdu = 1;
        //el.style.fontFamily = "Jameel Noori Nastaliq";
        //el.style.fontSize = pt;
        //el.style.backgroundColor = "Inherit";
       /* if ($(el).is("textarea")) {
            el.addClass("urdu-large");
        }
        else if ($(el).is("input"))
        {
            el.addClass("urdu-small");
        }*/
    }
    AddText = function (text) {
        if (!currEdit) return;
        var caretPos = currEdit.caretPos;
        if (currEdit.createTextRange && currEdit.caretPos) {
            caretPos.text = caretPos.text.charAt(caretPos.text.length - 1) == ' ' ?
            text + ' ' : text;
            currEdit.focus(caretPos);
        }
        else if (currEdit.selectionStart || currEdit.selectionStart == '0') {
            var vTop = currEdit.scrollTop;
            var startPos = currEdit.selectionStart;
            var endPos = currEdit.selectionEnd;
            currEdit.value = currEdit.value.substring(0, startPos) + text + currEdit.value.substring(endPos, currEdit.value.length);
            currEdit.focus();
            currEdit.selectionStart = startPos + 1;
            currEdit.selectionEnd = startPos + 1;
            currEdit.scrollTop = vTop;
        }
        else {
            currEdit.value += text;
            currEdit.focus(caretPos);
        }
    };
})(jQuery,document);