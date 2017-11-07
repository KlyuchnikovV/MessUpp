
var profileUrl = "http://localhost:49856/api/profile";
var chatUrl = "http://localhost:49856/api/chat";
var messageUrl = "http://localhost:49856/api/message";

// Post method. //
function CreateUser(object)
{
	var name = document.getElementById("txtName").value;
	var surname = document.getElementById("txtSurname").value;
	var login = document.getElementById("txtLogin").value;
	var password = document.getElementById("txtPassword").value;
	var request = new XMLHttpRequest();

	// Может понадобиться сменить адрес, добавить аватар и асинхронность. //

	request.open('POST', profileUrl, false);

	var user = '{ "Login" : "' + login + '", "Password" : "' + password + '", "Name" : "' + name + '", "Surname" : "' + surname + '", "Avatar" : [' + 0 + '] }';
	request.setRequestHeader("Content-type", "application/json");
	request.send(user);

	if (request.status != 200)
	{
		PopUp("Ошибка создания юзера: " + request.status + ': ' + request.statusText, 1, false);
		//alert( request.status + ': ' + request.statusText );
	}
	else
	{
		responseBody = request.responseText;
		var data = JSON.parse(responseBody);
		console.log(data);
	}
	//alert(name + surname + login + password);
}

// Post method. //
function Login()
{
    var login = document.getElementById("txtLogin").value;
    var password = document.getElementById("txtPassword").value;

    var request = new XMLHttpRequest();
	// Может понадобиться сменить адрес, добавить аватар и асинхронность. //
	request.open('POST', profileUrl + "/login", false);
	var user = '{ "Login" : "' + login + '", "Password" : "' + password + '" }';
	request.setRequestHeader("Content-type", "application/json");
	request.send(user);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			console.log(data);
			window.location = "main.html?id=" + data.Id;
			document.getElemehintById("profileId").value = data.Id;
			break;
		}
		default:
		{
			PopUp("Ошибка входа: " + request.status + ': ' + request.statusText, 1, false);			
			//alert( request.status + ': ' + request.statusText );
			break;
		}
	}


}

// Get method. //
function GetChats()
{
	var id = document.getElementById('profileId').value;
	var request = new XMLHttpRequest();
	var url = profileUrl + "/" + id + "/chats";
	request.open('GET', url, false);
	request.send(null);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			var mainList = document.getElementById("chatList");
			mainList.innerHTML = "";
			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var wrap = document.createElement("div");
				var but = document.createElement("div");
				var img = document.createElement("img");
				but.innerHTML = item.ChatName;
				wrap.setAttribute("class", "chatNodes");
				wrap.setAttribute("align", "left");
				but.setAttribute("onclick", "GetMessages()");
				but.setAttribute("style", "height=25px; width=calc(100% - 35px);display:inline-block; margin-left:10px; margin-top:10px; position:absolute");
				// If no pic then use default

				img.setAttribute("src", "./img/chatWithoutImage.png");
				img.setAttribute("style", "display:inline-block");
				img.setAttribute("height", "25px");
				img.setAttribute("width", "25px");
				img.setAttribute("vspace", "5px");
				img.setAttribute("hspace", "5px");

				wrap.appendChild(img);
				wrap.appendChild(but);
				mainList.appendChild(wrap);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка получения чатов: " + request.status + ': ' + request.statusText, 1, false);			
			//alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}

// Get method. //
function GetProfile(id)
{
	var request = new XMLHttpRequest();
	var url = profileUrl + "/" + id;
	request.open('GET', url, false);
	request.send(null);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			return data;
		}
		default:
		{
			PopUp("Ошибка получения юзера: " + request.status + ': ' + request.statusText, 1, false);
			//alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}

// Post method. //
function CreateChat()
{
	var id = document.getElementById('profileId').value;
	var chatName = document.getElementById("txtChatName").value;
	var request = new XMLHttpRequest();
	request.open('POST', chatUrl, false);
	var chat = '{ "ChatName" : "' + chatName + '", "ChatMembers" : [ { "Id" : "' + id + '" } ] }';
	request.setRequestHeader("Content-type", "application/json");
	request.send(chat);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			GetChats(id);
			break;
		}
		default:
		{
			PopUp("Ошибка создания чата: " + request.status + ': ' + request.statusText, 1, false);			
			//alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}

// Post method. //
function SendMessage()
{
	var profileId = document.getElementById('profileId').value;
	var chatId = document.getElementById("chatId").value;
	var messageText = document.getElementById("messageArea").value;
	if(messageText == "")
	{
		PopUp("Ошибка отправки сообщения: " + "пустое сообщение.", 1, false);		
		return;
	}
	var request = new XMLHttpRequest();
	request.open('POST', messageUrl, false);

	var message = '{ "ProfileId" : "' + profileId + '", "ChatId" : "' + chatId + '", "MessageText" : "' + messageText + '", "Attachment" : [' + 0 + '] } ';
	request.setRequestHeader("Content-type", "application/json");
	request.send(message);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			GetMessages();
			document.getElementById("messageArea").value = "";
			// Update dialog, clean message box, add attachments
			break;
		}
		default:
		{
			PopUp("Ошибка отправки: " + request.status + ': ' + request.statusText, 1, false);			
			//alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}


// Get method. //
function GetMessages()
{
	var chatId = document.getElementById("chatId").value;
	var request = new XMLHttpRequest();
	var url = messageUrl + "/chat/" + chatId;
	request.open('GET', url, false);
	request.send(null);

	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;

			var mainList = document.getElementById("dialog");
			mainList.innerHTML = "";
			var data = JSON.parse(responseBody);

			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var inf = document.createElement("div");
				var text = document.createElement("div");
				var message = document.createElement("div");
				var profile = GetProfile(item.ProfileId);
				inf.innerHTML = profile.Name + " " + profile.Surname;
				text.innerHTML = item.MessageText;
				text.setAttribute("class", "dialogMessage");
				inf.setAttribute("class", "messageInf");
				if(item.ProfileId == profile.Id)
				{
					text.setAttribute("style", 
						"float:right;display:block;width:95%; margin-bottom:5px; margin-right:5px");
					inf.setAttribute("style", 
						"float:right;display:block;width:95%; margin-top:5px;  margin-right:5px;color: #4169E1");
					message.setAttribute("style", 
						"float:right;margin-right:2%;display:block; width:50%; background-color: #0D0B15; -moz-border-radius: 10px;-webkit-border-radius: 9px; align-content: center; margin-top:10px;");
				}
				else
				{
					text.setAttribute("style", 
						"float:left;display:block;width:95%; margin-bottom:5px; margin-left:5px");
					inf.setAttribute("style", 
						"float:left;display:block;width:95%; margin-top:5px;  margin-left:5px; color:#FFBE33");
					message.setAttribute("style", 
						"float:left;margin-left:2%;display:block; width:50%; background-color: #1F1D10; -moz-border-radius: 10px;-webkit-border-radius: 9px; align-content: center; margin-top:10px;");					
				}
				message.appendChild(inf);
				message.appendChild(text);
				mainList.appendChild(message);
			}

			document.getElementById("messageBox").setAttribute("style",
				"visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:39px; position:absolute; bottom:5px; left: 0px; min-width:765px");
			document.getElementById("dialogMeta").setAttribute("style",
				"visibility:visible; position:absolute;height:35px;width:100%;top:0;background-color:#C0C0C0;padding-left:15px;padding-top:0px;bottom:10px");
			var chatData = GetChat(chatId);
			document.getElementById("dialogName").innerHTML = chatData.ChatName;
			document.getElementById("dialogName").setAttribute("style",
			"visibility:visible; position:absolute;height:35px;width:100%;top:0;padding-left:15px;margin-top:0px;bottom:10px");
			document.getElementById("dialog").setAttribute("style",
				"visibility:visible; text-align:left; margin-bottom:25px; padding:0; background:#202020; height:calc(100vh - 100px); width:(100% - 20px); position: absolute; top:35px; overflow-y:auto;");

			document.getElementById("dialog").scrollTop = document.getElementById("dialog").scrollHeight;

			break;
		}
		default:
		{
			PopUp("Ошибка получения сообщений: " + request.status + ': ' + request.statusText, 1, false);			
			//alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}

// Get method. //
function GetChat(id)
{
	var request = new XMLHttpRequest();
	var url = chatUrl + "/" + id;
	request.open('GET', url, false);
	request.send(null);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			return data;
		}
		default:
		{
			PopUp("Ошибка получения чата: " + request.status + ': ' + request.statusText, 1, false);			
			//alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}
