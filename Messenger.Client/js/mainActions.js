var $=require('jquery');

// Заполняем поле ИД профиля и загружам чаты пользователя. //
function OnLoad()
{
    var paramValue = window.location.href.split("?")[1].split("=")[1];
    document.getElementById("profileId").value = paramValue;
    GetChats();
    LoadProfileInfo();
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

    // Выводим сообщение о приветствии .//
    var id = document.getElementById("profileId").value;
    var profile = GetProfile(id);
    PopUp("Добро пожаловать, " + profile.Name + " " + profile.Surname + "!", 0, true);

    // Устанавливаем интервал обновления сообщений. //
    setInterval(
        function()
        {
            UpdateMessages();
        }
    , 1000);

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
        if ( file.type.match(/image.*/) ) 
        {
            document.getElementById('messageBox').setAttribute("style",
            "visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:159px; position:absolute; bottom:5px; left: 0px; min-width:765px");    

            var reader = new FileReader(), img;
            reader.addEventListener("load",
            function(event)
            {
                img = document.createElement('img');
                img.setAttribute("style", "top:5px;height:100px;width:100%;display:inline-block");
                img.setAttribute("id", "previewImage");
                img.src = event.target.result;
                divElem.appendChild(img);
            });
            reader.readAsDataURL(file);
        }
        else
        {
            document.getElementById('messageBox').setAttribute("style",
            "visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:159px; position:absolute; bottom:5px; left: 0px; min-width:765px");    

            var img;
            img = document.createElement('img');
            img.setAttribute("style", "margin-top:5px;height:100px;width:100%;display:inline-block");
            img.setAttribute("id", "previewImage");
            img.src = event.target.result;
            divElem.appendChild(img);
            switch(file.type)
            {
                case 'text/plain':
                case 'application/plain':
                {
                    img.src = "img/fileIcons/txtFile.png";
                    break;
                }
                case 'application/pdf':
                {
                    img.src = "img/fileIcons/pdfFile.png";
                    break;
                }
                case 'application/msword':
                {
                    img.src = "img/fileIcons/docFile.png";
                    break;
                }
                case 'audio/mpeg3':
                case 'audio/x-mpeg3':
                case 'video/mpeg':
                case 'video/x-mpeg':
                case 'audio/mp3':
                {
                    img.src = "img/fileIcons/mp3File.png";
                    break;
                }
                case 'application/zip':
                case 'application/x-compressed':
                case 'application/x-zip-compressed':
                {
                    img.src = "img/fileIcons/zipFile.png";
                    break;
                }
                case 'application/x-compress':
                {
                    img.src = "img/fileIcons/7zipFile.png";
                    break;
                }
                case 'application/rtf':
                case 'application/x-rtf':
                case 'text/richtext':
                {
                    img.src = "img/fileIcons/rtfFile.png";
                    break;
                }
                case 'application/mspowerpoint':
                case 'application/powerpoint':
                case 'application/vnd.ms-powerpoint':
                case 'application/x-mspowerpoint':
                {
                    img.src = "img/fileIcons/pptFile.png";
                    break;
                }
                case 'audio/mpg':
                case 'video/mpg':
                {
                    img.src = "img/fileIcons/mpgFile.png";
                    break;
                }
                case 'video/mp4':
                {
                    img.src = "img/fileIcons/mp4File.png";
                    break;
                }
                case 'audio/wav':
                case 'audio/x-wav':
                {
                    img.src = "img/fileIcons/wavFile.png";
                    break;
                }
                case 'audio/octet-stream':
                {
                    img.src = "img/fileIcons/exeFile.png";
                    break;
                }
                default:
                {
                    img.src = "img/fileIcons/file.png";
                    break;
                }
            }
        } 
    }

    // Инициализируем автозаполнение. //
    //var options = {
    //    data: ["blue", "green", "pink", "red", "yellow"]
    //};
    
    //$("#txtChatName").easyAutocomplete(options);
    //$("#findString").easyAutocomplete(options);
    //$("#messageArea").easyAutocomplete(options);
});	

// Функция показа всплывающих сообщений. //
function PopUp(message, type, isAutoClose)
{
    var popUp = document.getElementById("popUp");
    var popUpMessage = document.getElementById("popUpMessage");
    popUpMessage.innerHTML = message;
    switch(type)
    {
        case 0:
        {
            popUp.setAttribute("style", 
                "background:#808080;z-index:99;visibility:visible;color:#018813; height:40px; " + 
                "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; " + 
                "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
            break;
        }
        case 1:
        {
            popUp.setAttribute("style", 
                "background:#808080;z-index:99;visibility:visible;color:#a30101; height:40px; " + 
                "width:50vw; position:absolute; left:25vw;padding-top:20px; top:-60px; " + 
                "-moz-border-radius: 10px; -webkit-border-radius: 9px; text-align:center");
            break;
        }
    }
    $("div.popUp").animate({top:'0px'},500);
    var img = document.createElement("img");
    img.setAttribute("src", "./img/close.png");
    img.setAttribute("height", "25px");
    img.setAttribute("width", "25px");
    img.setAttribute("vspace", "5px");
    img.setAttribute("hspace", "5px");
    img.setAttribute("style", "position:absolute; right:0px; top:0px");
    img.setAttribute("onclick", "ClosePopUp()");
    img.setAttribute("class", "divButton");
    popUp.appendChild(img);
    if(isAutoClose)
    {
        setTimeout( function()
        {
            $("div.popUp").animate({top:'-60px'},500);
            setTimeout( function()
            {
                popUp.setAttribute("style", 
                    "color:#FFFFFF; height:40px; width:50vw; position:absolute;padding-top:20px; left:25vw;-moz-border-radius: 10px; -webkit-border-radius: 9px; top:-60px;background-color:#53472A;");
            }, 500);
        }, 5000);
    }
}

// Функция закрытия всплывающего сообщения. //
function ClosePopUp()
{
    $("div.popUp").animate({top:'-60px'},500);
}


