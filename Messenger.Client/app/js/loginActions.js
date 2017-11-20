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

