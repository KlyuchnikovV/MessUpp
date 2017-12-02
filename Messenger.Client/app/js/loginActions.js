var $ = require('jquery');
function OnLoadLogin()
{
    // Функция переключения состояния боковой панели чатов. //
    $("div.divButtonLogin").click(
        function()
        {
            ResetPanels(document.getElementById("loginHidden"));

            if(document.getElementById("loginHidden").value == "false")
            {
                document.getElementById("loginHidden").value = "true";
                $("div.loginPanel").animate({left:'235px'},500);
                $('div.introPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.introPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.loginPanel").animate({left:-35},500);
                $("div.introPanel").animate({left:35},500);
                $('div.introPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("loginHidden").value = "false";
            }
        }
    );
    // Функция переключения состояния боковой панели создания чата. //
    $("div.divButtonRegister").click(
        function()
        {
            ResetPanels(document.getElementById("registerHidden"));

            if(document.getElementById("registerHidden").value == "false")
            {
                document.getElementById("registerHidden").value = "true";
                $("div.registerPanel").animate({left:'200px'},500);
                $('div.introPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.introPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.registerPanel").animate({left:0},500);
                $("div.introPanel").animate({left:35},500);
                $('div.introPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("registerHidden").value = "false";
            }
        }
    );

    var textboxes = document.getElementsByTagName('input');

    document.getElementById('avatarFile').onchange = function (evt) {
        var tgt = evt.target || window.event.srcElement,
            files = tgt.files;

        if (FileReader && files && files.length)
        {
            var fr = new FileReader();
            fr.onload = function ()
            {
                document.getElementById("avatar").src = fr.result;
                var close = document.createElement('img');
                close.setAttribute("src", "./img/close.png");
                close.setAttribute("height", "25px");
                close.setAttribute("width", "25px");
                close.setAttribute("vspace", "5px");
                close.setAttribute("hspace", "5px");
                close.setAttribute("style", "z-index:99;background-color:transparent;position:absolute;height:25px;width:25px;left:160px;top:80px;");
                close.setAttribute("onclick", "DeleteAvatar()");
                close.setAttribute("class", "divButton");
                document.getElementById("avatarDiv").appendChild(close);
            }
            fr.readAsDataURL(files[0]);
        }
        else
        {
            PopUp("Не могу загрузить фотографию.", 1, false);
        }
    }

    PopUp("Добро пожаловать в MessUpp!", 0, true);
    document.getElementById("loginDiv").click();
}

// Функция закрытия всех боковых панелей. //
function ResetPanels(element)
{
    if(document.getElementById("loginHidden").value != "false" && element != document.getElementById("loginHidden"))
    {
        $("div.loginPanel").animate({left:0},500);
        document.getElementById("loginHidden").value = "false";
    }
    if(document.getElementById("registerHidden").value != "false" && element != document.getElementById("registerHidden"))
    {
        $("div.registerPanel").animate({left:0},500);
        document.getElementById("registerHidden").value = "false";
    }
}

function DeleteAvatar()
{
    document.getElementById('avatarDiv').innerHTML = "";
    document.getElementById('avatarDiv').innerHTML = '<img id="avatar" style="max-height:200px; max-width:150px" src="./img/personWithoutImage.png"/>';
    document.getElementById('avatarFile').value = "";
}