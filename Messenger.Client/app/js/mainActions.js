var $=require('jquery');

// Заполняем поле ИД профиля и загружам чаты пользователя. //
function OnLoad()
{
    var paramValue = window.location.href.split("?")[1].split("=")[1];
    document.getElementById("profileId").value = paramValue;
    GetChats();
    LoadProfileInfo();
    // Выводим сообщение о приветствии .//
    var id = document.getElementById("profileId").value;
    var profile = GetProfile(id);
    PopUp("Добро пожаловать, " + profile.Name + " " + profile.Surname + "!", 0, true);
}

// Функция закрытия всех боковых панелей. //
function ResetPanels(element)
{
    if(document.getElementById("chatsHidden").value != "false" && element != document.getElementById("chatsHidden"))
    {
        $("div.chatsPanel").animate({left:0},500);
        document.getElementById("chatsHidden").value = "false";
    }
    if(document.getElementById("createHidden").value != "false" && element != document.getElementById("createHidden"))
    {
        $("div.createPanel").animate({left:0},500);
        document.getElementById("createHidden").value = "false";
    }
    if(document.getElementById("findHidden").value != "false" && element != document.getElementById("findHidden"))
    {
        $("div.findPanel").animate({left:0},500);
        document.getElementById("findHidden").value = "false";
    }
    if(document.getElementById("settingsHidden").value != "false" && element != document.getElementById("settingsHidden"))
    {
        $("div.settingsPanel").animate({left:0},500);
        document.getElementById("settingsHidden").value = "false";
    }
    if(document.getElementById("peopleHidden").value != "false" && element != document.getElementById("peopleHidden"))
    {
        $("div.peoplePanel").animate({left:0},500);
        document.getElementById("peopleHidden").value = "false";
    }
    if(document.getElementById("logoutHidden").value != "false" && element != document.getElementById("logoutHidden"))
    {
        $("div.logoutPanel").animate({left:0},500);
        document.getElementById("logoutHidden").value = "false";
    }
}

// Инициализация приложения. //
$(document).ready(function(){

    // Функция переключения состояния боковой панели чатов. //
    $("div.divButtonChats").click(
        function()
        {
            ResetPanels(document.getElementById("chatsHidden"));

            if(document.getElementById("chatsHidden").value == "false")
            {
                document.getElementById("chatsHidden").value = "true";
                $("div.chatsPanel").animate({left:'235px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.chatsPanel").animate({left:-35},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("chatsHidden").value = "false";
            }
        }
    );

    // Функция переключения состояния боковой панели создания чата. //
    $("div.divButtonCreate").click(
        function()
        {
            ResetPanels(document.getElementById("createHidden"));

            if(document.getElementById("createHidden").value == "false")
            {
                document.getElementById("createHidden").value = "true";
                $("div.createPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.createPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("createHidden").value = "false";
            }
        }
    );

    // Функция переключения состояния боковой панели поиска. //
    $("div.divButtonFind").click(
        function()
        {
            ResetPanels(document.getElementById("findHidden"));

            if(document.getElementById("findHidden").value == "false")
            {
                document.getElementById("findHidden").value = "true";
                $("div.findPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.findPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("findHidden").value = "false";
            }
        }
    );

    // Функция переключения состояния боковой панели настроек. //
    $("div.divButtonSettings").click(
        function()
        {
            ResetPanels(document.getElementById("settingsHidden"));

            if(document.getElementById("settingsHidden").value == "false")
            {
                document.getElementById("settingsHidden").value = "true";
                $("div.settingsPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)',

                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.settingsPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("settingsHidden").value = "false";
            }
        }
    );

    // Функция переключения состояния боковой панели пользователей чата. //
    $("div.divButtonPeople").click(
        function()
        {
            ResetPanels(document.getElementById("peopleHidden"));

            if(document.getElementById("peopleHidden").value == "false")
            {
                document.getElementById("peopleHidden").value = "true";
                $("div.peoplePanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)',

                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.peoplePanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("peopleHidden").value = "false";
            }
        }
    );

    // Функция переключения состояния боковой панели выхода. //
    $("div.divButtonLogout").click(
        function()
        {
            ResetPanels(document.getElementById("logoutHidden"));

            if(document.getElementById("logoutHidden").value == "false")
            {
                document.getElementById("logoutHidden").value = "true";
                $("div.logoutPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)',

                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.logoutPanel").animate({left:0},500);
                $("div.dialogPanel").animate({left:35},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)',
                    }
                );
                document.getElementById("logoutHidden").value = "false";
            }
        }
    );

    // Устанавливаем интервал обновления сообщений. //
    setInterval(
        function()
        {
            if(CountReadMessages(document.getElementById("chatId").value))
            {
                GetMessages(divs[i].value);
            } 
            var divs = document.getElementsByClassName("chatIdDiv");
            for(var i = 0; i < divs.length; i++)
            {
                if(UpdateMessages(divs[i].value))
                {
                    if(document.getElementById("chatId").value == divs[i].value)
                    {
                        GetMessages(divs[i].value);
                        continue;
                    }
                    else if(document.getElementById("chatId").value  != "")
                    {
                        var message = GetLastMessage(divs[i].value);
                        if(message.ProfileId != document.getElementById("profileId").value && message.IsRead == false)
                            MessagePopUp(message);
                    }
                }
            }
        }
    , 1000);

    setInterval(
      function()
      {
        if(document.getElementById('chatId').value != "")
        {
          ChatProfiles();
          GetChats();
        }
      }, 30000
    );

    // Инициализируем превью файлов. //
    var inpElem = document.getElementById('file-input'),
        divElem = document.getElementById('preview');
    inpElem.addEventListener("change", function(e) {
        preview(this.files[0]);
    });
    function preview(file)
    {
        divElem.innerHTML = "";
        inpElem.innerHTML = inpElem.innerHTML;
        document.getElementById('messageBox').setAttribute("style",
        "visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:159px; position:absolute; bottom:5px; left: 0px; min-width:200px");

        var reader = new FileReader(), img;
        reader.addEventListener("load",
            function(event)
            {
                img = document.createElement('img');
                var close = document.createElement('img');
                close.setAttribute("src", "./img/close.png");
                close.setAttribute("height", "25px");
                close.setAttribute("width", "25px");
                close.setAttribute("vspace", "5px");
                close.setAttribute("hspace", "5px");
                close.setAttribute("style", "background-color:transparent;position:absolute;margin-right:5px;margin-left:-30px;height:25px;width:25px;top:0px;z-index:90;");
                close.setAttribute("onclick", "DeleteAttach()");
                close.setAttribute("class", "divButton");
                img.setAttribute("style", "top:5px;height:100px;width:100%;display:inline-block;");
                img.setAttribute("id", "previewImage");
                img.src = event.target.result;
                divElem.appendChild(img);
                divElem.appendChild(close);
            });
        reader.readAsDataURL(file);
    }
});

// Вадидация таймера. //
function TimerValidator(input)
{
    var value = input.value;
    var rep = /^\d+$/;
    if(!rep.test(value))
    {
        input.value = "0";
    }
}

// Выводит информацию о профиле в раздел настроек. //
function LoadProfileInfo()
{
	var profileId = document.getElementById("profileId").value;
    var item = GetProfile(profileId);
    var img = document.getElementById("avatar");
    document.getElementById("txtName").value = item.Name;
    document.getElementById("txtSurname").value = item.Surname;
    document.getElementById("txtLogin").value = item.Login;
	if(item.Avatar != null)
	{
		var attachData = GetAttachData(item.Avatar);
		img.setAttribute("src", 'data:image/jpeg;base64,' + attachData.Data);
	}
	else
	{
		img.setAttribute("src", "./img/personWithoutImage.png");
    }
}

async function SelfDestroy(timeToUpdate, chatId)
{
	setTimeout(
		function()
		{
			GetMessages(chatId);
		}, timeToUpdate * 1000);
}

function DeleteAttach()
{
    document.getElementById('preview').innerHTML = "";
    document.getElementById('file-input').value = "";
    document.getElementById('messageBox').setAttribute("style",
    "visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:39px; position:absolute; bottom:5px; left: 0px; min-width:200px");
}

function ChatValidate(input)
{
    var value = input.value;
    var rep = /^[a-zA-Zа-яА-Я0-9\ ]+$/;
    if(!rep.test(value))
    {
        //input.value = "";
        input.setAttribute("style", "border:1px solid #CD853F;color:red;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");
        document.getElementById("chatPassed").value = false;
    }
    else
    {
        document.getElementById("chatPassed").value = true;
        input.setAttribute("style", "border:1px solid #CD853F;color:green;-moz-border-radius: 10px;-webkit-border-radius: 9px;padding-left: 5px;font-weight: bold;");

    }
}

function Response(id)
{
    var profile = GetProfile(id);
    document.getElementById("messageArea").value += "@" + profile.Login + ", ";
}

// Функция показа всплывающих сообщений. //
function MessagePopUp(message)
{
    var popUp = document.getElementById("messagePopUp");
    var popUpInf = document.getElementById("messagePopUpInf");
    var popUpMessage = document.getElementById("messagePopUpMessage");
    var profile = GetProfile(message.ProfileId);
    var chat = GetChat(message.ChatId);
    popUpInf.innerHTML = profile.Name + " " + profile.Surname + " из чата " + chat.ChatName;
    popUpMessage.innerHTML = message.MessageText;

    popUp.setAttribute("style",
        "background:#808080;z-index:99;visibility:visible;color:#ffffff; height:60px; " +
        "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-100px; " +
        "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
    setTimeout( function()
    {
        $("div#messagePopUp").animate({top:'-100px'},500);
        setTimeout( function()
        {
            popUp.setAttribute("style",
                "background:#808080;z-index:99;visibility:visible;height:60px; " +
                "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-100px; " +
                "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center;color:#ffffff");
        }, 500);
    }, 5000);
    $("div#messagePopUp").animate({top:'0px'},500);
    var img = document.createElement("img");
    img.setAttribute("src", "./img/close.png");
    img.setAttribute("height", "25px");
    img.setAttribute("width", "25px");
    img.setAttribute("vspace", "5px");
    img.setAttribute("hspace", "5px");
    img.setAttribute("style", "position:absolute; right:0px; top:0px");
    img.setAttribute("onclick", "CloseMessagePopUp()");
    img.setAttribute("class", "divButton");
    popUp.appendChild(img);
    
}

function CloseMessagePopUp()
{
    $("div#messagePopUp").animate({top:'-100px'},500);
}

function ChangeState()
{
    var checkbox = document.getElementById("checkbox");
    var input = document.getElementById("timer");
    if(checkbox.checked)
    {
        //checkbox.checked= false;
        input.disabled = false;
        input.value = 10;
    }
    else
    {
        //checkbox.checked = true;
        input.disabled = true;
        input.value = 0;
    }
}