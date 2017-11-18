var $=require('jquery');
function OnLoad() 
{
    var paramValue = window.location.href.split("?")[1].split("=")[1];
    document.getElementById("profileId").value = paramValue;
    GetChats();
}
function OnRedirect(whereTo)
{
    document.location = whereTo + "?id=" + document.getElementById("profileId").value;
}



$(document).ready(function(){	
    $("div.divButtonChats").click(
        function()
        {
            if(document.getElementById("createHidden").value != "false")
            {
                $("div.createPanel").animate({left:0},500);
                document.getElementById("createHidden").value = "false";
            }
            if(document.getElementById("findHidden").value != "false")
            {
                $("div.findPanel").animate({left:0},500);
                document.getElementById("findHidden").value = "false";
            }
            if(document.getElementById("settingsHidden").value != "false")
            {
                $("div.settingsPanel").animate({left:0},500);
                document.getElementById("settingsHidden").value = "false";
            }

            if(document.getElementById("chatsHidden").value == "false")
            {
                document.getElementById("chatsHidden").value = "true";
                $("div.chatsPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 236px)'
                    }
                );
                $("div.dialogPanel").animate({left:'235px'},500);
            }
            else
            {
                $("div.chatsPanel").animate({left:0},500);
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

    $("div.divButtonCreate").click(
        function()
        {
            if(document.getElementById("chatsHidden").value != "false")
            {
                $("div.chatsPanel").animate({left:0},500);
                document.getElementById("chatsHidden").value = "false";
            }
            if(document.getElementById("findHidden").value != "false")
            {
                $("div.findPanel").animate({left:0},500);
                document.getElementById("findHidden").value = "false";
            }
            if(document.getElementById("settingsHidden").value != "false")
            {
                $("div.settingsPanel").animate({left:0},500);
                document.getElementById("settingsHidden").value = "false";
            }

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

    $("div.divButtonFind").click(
        function()
        {
            if(document.getElementById("chatsHidden").value != "false")
            {
                $("div.chatsPanel").animate({left:0},500);
                document.getElementById("chatsHidden").value = "false";
            }
            if(document.getElementById("createHidden").value != "false")
            {
                $("div.createPanel").animate({left:0},500);
                document.getElementById("createHidden").value = "false";
            }
            if(document.getElementById("settingsHidden").value != "false")
            {
                $("div.settingsPanel").animate({left:0},500);
                document.getElementById("settingsHidden").value = "false";
            }

            if(document.getElementById("findHidden").value == "false")
            {
                document.getElementById("findHidden").value = "true";
                $("div.findPanel").animate({left:'200px'},500);
                $('div.dialogPanel').css(
                    {
                        'width' : 'calc(100vw - 36px)'
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
                        'width' : 'calc(100vw - 236px)',
                    }
                );
                document.getElementById("findHidden").value = "false";
            }
        }
    );

    $("div.divButtonSettings").click(
        function()
        {
            if(document.getElementById("chatsHidden").value != "false")
            {
                $("div.chatsPanel").animate({left:0},500);
                document.getElementById("chatsHidden").value = "false";
            }
            if(document.getElementById("createHidden").value != "false")
            {
                $("div.createPanel").animate({left:0},500);
                document.getElementById("createHidden").value = "false";
            }
            if(document.getElementById("findHidden").value != "false")
            {
                $("div.findPanel").animate({left:0},500);
                document.getElementById("findHidden").value = "false";
            }

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

});	

// по нажатию Enter в поле ввода текста
$('messageArea').onkeydown = 
    function (e) 
    {
        // если человек нажал Ctrl+Enter или Shift+Enter, то просто создаем новую строку. 
        if (e.which == 13 && !e.ctrlKey && !e.shiftKey) 
        {
            // отправляем серверу событие message
            SendMessage(); 
            $('messageArea').innerText = ''; // чистим поле ввода
        }
    }
// скроллим вниз при новом сообщении
var observer = new MutationObserver(function(mutations) {
	mutations.forEach(function(mutation) {
		var objDiv = $('messages');
		objDiv.scrollTop = objDiv.scrollHeight;
	}); 
}).observe($('messages'), { childList: true });