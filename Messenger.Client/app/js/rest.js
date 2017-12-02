const fs = require('fs');

// Пути к контроллерам. //
var profileUrl    = "http://localhost:49856/api/profile";
var chatUrl       = "http://localhost:49856/api/chat";
var messageUrl    = "http://localhost:49856/api/message";
var attachmentUrl = "http://localhost:49856/api/attach";

/// Profile methods. ///

// Post method. Async. //
//  Отправляет запрос на создание нового профиля. //
function CreateUser(id)
{
	DisableDiv(document.getElementById("registerPan"), 200, "px", 100, "vh");
	if(document.getElementById("registerPassed").value == 'false')
	{
		PopUp("Валидация полей не пройдена.", 1, false);
		EnableDiv(document.getElementById("registerPan"));
		return;
	}

	var name = document.getElementById("txtName").value;
	var surname = document.getElementById("txtSurname").value;
	var login = document.getElementById("txtLogin").value;
	var password = document.getElementById("txtPassword").value;
	var request = new XMLHttpRequest();
	request.open('POST', profileUrl, true);
	var user =
		'{ "Login" : "' + login + '", "Password" : "' + password + '", "Name" : "' +
			name + '", "Surname" : "' + surname + '", "Avatar" : "' + id + '" }';

	request.setRequestHeader("Content-type", "application/json");

	try
	{
		request.send(user);
	}
	catch(Exception)
	{
		PopUp("Ошибка осоздания профиля: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("registerPan"));
		return;
	}
	request.onreadystatechange = function()
	{
		if(request.readyState == 4)
		{
			switch(request.status)
			{
				case 200:
				{
					PopUp("Профиль успешно создан!", 0, true);
					break;
				}
				case 409:
				{
					PopUp("Ошибка создания профиля: профиль с данным логином уже существует.", 1, false);
					break;
				}
				default:
				{
					PopUp("Ошибка создания профиля: " + request.status + ': ' + request.statusText, 1, false);
				}
			}
			EnableDiv(document.getElementById("registerPan"));
		}
	};
}

// Get method. Sync. //
// Отправляет запрос на полуение пользователя по ИД. //
function GetProfile(id)
{
	var request = new XMLHttpRequest();
	var url = profileUrl + "/" + id;
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения профиля: нет подключения к серверу", 1, false);
		return;
	}
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
			PopUp("Ошибка получения профиля: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// 
function UpdateProfile()
{
	if(!confirm("Обновить профиль с этими данными?"))
	{
		return;
	}

	DisableDiv(document.getElementById("settingsPanel"), 200, "px", 100, "vh");
	if(document.getElementById("registerPassed").value == 'false')
	{
		PopUp("Валидация полей не пройдена.", 1, false);
		EnableDiv(document.getElementById("settingsPanel"));
		return;
	}

	var id = document.getElementById("profileId").value;
	var name = document.getElementById("txtName").value;
	var surname = document.getElementById("txtSurname").value;
	var login = document.getElementById("txtLogin").value;
	var password = document.getElementById("txtPassword").value;
	var request = new XMLHttpRequest();
	request.open('POST', profileUrl + "/update", true);
	var user =
		'{ "Id" : "' + id + '", "Login" : "' + login + '", "Password" : "' + password + '", "Name" : "' +
			name + '", "Surname" : "' + surname + '" }';

	request.setRequestHeader("Content-type", "application/json");

	try
	{
		request.send(user);
	}
	catch(Exception)
	{
		PopUp("Ошибка обновления: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("settingsPanel"));
		return;
	}

	request.onreadystatechange = function()
	{
		if(request.readyState == 4)
		{
			switch(request.status)
			{
				case 200:
				{
					PopUp("Профиль обновлен!", 0, true);
					break;
				}
				case 409:
				{
					PopUp("Ошибка обновления: профиль с данным логином уже существует.", 1, false);
					break;
				}
				default:
				{
					PopUp("Ошибка обновления: " + request.status + ': ' + request.statusText + " " + request.Message, 1, false);
				}
			}
			EnableDiv(document.getElementById("settingsPanel"));
		}
	};
}

// Delete method. Async. //
// Отправляет запрос на удаление профиля по ИД. //
function DeleteProfile()
{
	DisableDiv(document.getElementById("settingsPanel"), 200, "px", 100, "vh");
	var request = new XMLHttpRequest();
	var id = document.getElementById("profileId").value;
	var url = profileUrl + "/" + id;
	request.open('DELETE', url, true);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка удаления профиля: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("settingsPanel"));
		return;
	}
	request.onreadystatechange = function()
	{
		if(request.readyState == 4)
		{
			switch(request.status)
			{
				case 200:
				case 204:
				{
					PopUp("Профиль успешно удален.", 0, true);
					window.location = "login.html";
					break;
				}
				default:
				{
					PopUp("Ошибка удаления профиля: " + request.status + ': ' + request.statusText, 1, false);
					break;
				}
			}
			EnableDiv(document.getElementById("settingsPanel"));
		}
	}
}

// Get method. Sync. //
// Отправляет запрос на получение чатов пользователя. //
function GetLogin(login)
{
	var request = new XMLHttpRequest();
	var url = profileUrl + "/find/login";
	request.open('POST', url, false);
	var user =
	'{ "tokens" : [ "' + login + '"] }';

	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(user);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения логина: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("chatsPanel"));
		return;
	}
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
			PopUp("Ошибка получения логина: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Get method. Sync. //
// Отправляет запрос на получение чатов пользователя. //
function GetChats()
{
	var id = document.getElementById('profileId').value;
	var request = new XMLHttpRequest();
	var url = profileUrl + "/" + id + "/chats";
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения чатов: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("chatsPanel"));
		return;
	}
	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			var mainList = document.getElementById("chatList");
			if(data.length > 0)
				mainList.innerHTML = "";
			else
				mainList.innerHTML = '<label class="label" style="color:#ffffff">Здесь пока нет ни одного чата:(</label>';
			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var wrap = document.createElement("div");
				var but = document.createElement("div");

				var chatIdDiv = document.createElement("input");
				var chatIdRead = document.createElement("input");

				chatIdRead.setAttribute("type", "hidden");
				chatIdRead.setAttribute("id", "read" + item.ChatId);
				chatIdDiv.setAttribute("type", "hidden");
				chatIdDiv.setAttribute("value", item.ChatId);
				var hidden = document.createElement("input");
				hidden.setAttribute("type", "hidden");
				hidden.setAttribute("id", "count" + item.ChatId);
				chatIdDiv.setAttribute("class", "chatIdDiv");
				wrap.appendChild(hidden);
				wrap.appendChild(chatIdDiv);
				wrap.appendChild(chatIdRead);

				var close = document.createElement("img");
				var img = document.createElement("img");
				but.innerHTML = item.ChatName;
				wrap.setAttribute("class", "chatNodes");
				wrap.setAttribute("align", "left");
				but.setAttribute("onclick", 'GetMessages("' + item.ChatId + '")');
				but.setAttribute("style", "height=25px; width=calc(100% - 35px);" +
										"display:inline-block; margin-left:10px; margin-top:10px; position:absolute");
				img.setAttribute("src", "./img/chatWithoutImage.png");
				img.setAttribute("style", "display:inline-block");
				img.setAttribute("height", "25px");
				img.setAttribute("width", "25px");
				img.setAttribute("vspace", "5px");
				img.setAttribute("hspace", "5px");
				close.setAttribute("src", "./img/close.png");
				close.setAttribute("height", "25px");
				close.setAttribute("width", "25px");
				close.setAttribute("vspace", "5px");
				close.setAttribute("hspace", "5px");
				close.setAttribute("style", "position:absolute;margin-right: 5px;margin-left:110px");
				close.setAttribute("onclick", "DeleteChat('" + item.ChatId + "')");
				close.setAttribute("class", "divButton");
				wrap.appendChild(img);
				wrap.appendChild(but);
				wrap.appendChild(close);
				mainList.appendChild(wrap);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка получения чатов: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Post method. Async. //
// Отправляет запрос на поиск профиля по логину и паролю. //
function Login()
{
	DisableDiv(document.getElementById("loginPan"), 200, "px", 100, "vh");
	if(document.getElementById("loginPassed").value == "false")
	{
		PopUp("Валидация полей не пройдена.", 1, false);
		EnableDiv(document.getElementById("loginPan"));
		return;
	}
  	var login = document.getElementById("txtLog").value;
  	var password = document.getElementById("txtPass").value;
	var request = new XMLHttpRequest();
	request.open('POST', profileUrl + "/login", true);
	var user = '{ "Login" : "' + login + '", "Password" : "' + password + '" }';
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(user);
	}
	catch(Exception)
	{
		PopUp("Ошибка входа: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("loginPan"));
		return;
	}

	request.onreadystatechange = function()
	{
		if(request.readyState == 4)
		{
			switch(request.status)
			{
				case 200:
				{
					responseBody = request.responseText;
					var data = JSON.parse(responseBody);
					EnableDiv(document.getElementById("loginPan"));
					window.location = "main.html?id=" + data.Id;
					break;
				}
				case 203:
				{
					PopUp("Ошибка входа: данная пара логин/пароль не существует.", 1, false);
					EnableDiv(document.getElementById("loginPan"));
					break;
				}
				case 0:
				{
					PopUp("Ошибка входа: нет подключения к серверу", 1, false);
					EnableDiv(document.getElementById("loginPan"));
					break;
				}
				default:
				{
					PopUp("Ошибка входа: " + request.status + ': ' + request.statusText, 1, false);
					EnableDiv(document.getElementById("loginPan"));
					break;
				}
			}
		}
	};
}

// Get method. Async. //
// Отправляет запрос на установку флага "в сети" в значение "не в сети". //
function Logout()
{
	DisableDiv(document.getElementById("logoutPan"), 200, "px", 100, "vh");
	var id = document.getElementById("profileId").value;
	var request = new XMLHttpRequest();
	request.open('GET', profileUrl + "/logout/" + id, true);
	try
	{
		request.send(user);
	}
	catch(Exception)
	{
		PopUp("Ошибка выхода: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("logoutPan"));
		return;
	}
}

/// Chat methods. ///

// Post method. Async. //
// Отправляет запрос на создание чата. //
function CreateChat()
{
	if(document.getElementById("chatPassed").value != "true")
	{
		PopUp("Неверное название чата", 1, false);
		return;
	}
	DisableDiv(document.getElementById("createPanel"), 200, "px", 100, "vh");
	var id = document.getElementById('profileId').value;
	var chatName = document.getElementById("txtChatName").value;
	var request = new XMLHttpRequest();
	request.open('POST', chatUrl, true);
	var chat = '{ "ChatName" : "' + chatName + '", "ChatMembers" : [ { "Id" : "' + id + '" } ] }';
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(chat);
	}
	catch(Exception)
	{
		PopUp("Ошибка создания чата: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("createPanel"));
		return;
	}
	request.onreadystatechange = function()
	{
		if(request.readyState == 4)
		{
			switch(request.status)
			{
				case 200:
				{
					responseBody = request.responseText;
					var data = JSON.parse(responseBody);
					GetChats(id);
					document.getElementById("chatId").value = data.ChatId;
					document.getElementById("chats").click();
					GetMessages(data.ChatId);
					PopUp("Чат создан!", 0, true);
					break;
				}
				default:
				{
					PopUp("Ошибка создания чата: " + request.status + ': ' + request.statusText, 1, false);
					break;
				}
			}
			EnableDiv(document.getElementById("createPanel"));
		}
	}
}

// Get method. Sync. //
// Отправляет запрос на получение чата по ИД. //
function GetChat(id)
{
	var request = new XMLHttpRequest();
	var url = chatUrl + "/" + id;
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения чата: нет подключения к серверу", 1, false);
		return;
	}

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
			break;
		}
	}
}

// Delete method. Sync. //
// Отправляет запрос на удаление чата по ИД. //
function DeleteChat(id)
{
	DisableDiv(document.getElementById("chatsPanel"), 200, "px", 100, "vh");
	var request = new XMLHttpRequest();
	var url = chatUrl + "/" + id;
	request.open('DELETE', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка удаления чата: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("chatsPanel"));
		return;
	}
	switch(request.status)
	{
		case 200:
		case 204:
		{
			PopUp("Чат успешно удален.", 0, true);
			EnableDiv(document.getElementById("chatsPanel"));
			GetChats();
			break;
		}
		default:
		{
			PopUp("Ошибка удаления чата: " + request.status + ': ' + request.statusText, 1, false);
			EnableDiv(document.getElementById("chatsPanel"));
			break;
		}
	};
}

// Get method. Sync. //
// Отправляет запрос на добавление выбранного пользователя к текущему чату. //
function AddToChat(id)
{
	var request = new XMLHttpRequest();
	var chatId = document.getElementById("chatId").value;
	var profile = GetProfile(id);
	if(!confirm("Подтверждение добавления " + profile.Name + " " + profile.Surname + " к диалогу."))
	{
		return;
	}

	if(chatId == "")
	{
		PopUp("Ошибка добавления к чату: не выбран чат", 1, false);
		return;
	}
	var url = chatUrl + "/" + chatId + "/add/profile/" + id;
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка добавления к чату: нет подключения к серверу", 1, false);
		return;
	}

	switch(request.status)
	{
		case 200:
		case 204:
		{
			PopUp("Профиль успешно добавлен в чат!", 0, true);
			ChatProfiles();
			break;
		}
		case 404:
		{
			PopUp("Вы не можете еще раз добавить этот профиль!", 1, true);
			break;
		}
		default:
		{
			PopUp("Ошибка добавления: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Delete method. Sync. //
// Отправляет запрос на удаление пользователя чата по ИД. //
function DeleteChatProfile()
{
	var request = new XMLHttpRequest();
	var chatId = document.getElementById("chatId").value;
	var id = document.getElementById("profileId").value;
	var url = chatUrl + "/" + chatId + "/delete/profile/" + id;
	request.open('DELETE', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка удаления профиля из чата: нет подключения к серверу", 1, false);
		return;
	}

	switch(request.status)
	{
		case 200:
		case 204:
		{
			PopUp("Профиль успешно удален из чата.", 0, true);
			document.getElementById("messageBox").setAttribute("style",
			"visibility:hidden;background-color:#202020; vertical-align:center; width:100%; height:39px; position:absolute; bottom:5px; left: 0px; min-width:200px");
			document.getElementById("dialogMeta").setAttribute("style",
				"visibility:hidden; position:absolute;height:35px;width:100%;top:0;background-color:#C0C0C0;padding-left:15px;padding-top:0px;bottom:10px");
			document.getElementById("dialogName").setAttribute("style",
			"visibility:hidden; position:absolute;height:35px;width:100%;top:0;padding-left:15px;margin-top:0px;bottom:10px");
			document.getElementById("dialog").setAttribute("style",
				"visibility:hidden; text-align:left; margin-bottom:25px; padding:0; background:#202020; height:calc(100vh - 150px); width:(100% - 20px); position: absolute; top:35px; overflow-y:auto;");
			GetChats();
			return;
		}
		default:
		{
			PopUp("Ошибка удаления профиля из чата: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Get method. Sync. //
// Отправлет запрос на получение профилей состоящих в текущем чате. //
function ChatProfiles()
{
	var id = document.getElementById('chatId').value;
	var request = new XMLHttpRequest();
	var url = chatUrl + "/" + id + "/get/profiles";
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения пользователей: нет подключения к серверу", 1, false);
		return;
	}

	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			var mainList = document.getElementById("chatProfiles");
			mainList.innerHTML = "";
			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var wrap = document.createElement("div");
				var but = document.createElement("div");
				var img = document.createElement("img");
				var online = document.createElement("img");
				but.innerHTML = item.Name + " " + item.Surname;
				//alert(item.IsOnline);
				if(item.IsOnline)
				{
					online.setAttribute("src", "./img/online.png");
					online.setAttribute("style", "position:absolute;");
					online.setAttribute("height", "5px");
					online.setAttribute("width", "5px");
					online.setAttribute("vspace", "25px");
					online.setAttribute("hspace", "25px");
					wrap.appendChild(online);
				}
				// Set online/offline image
				wrap.setAttribute("title", item.Name + " " + item.Surname);
				wrap.setAttribute("class", "chatNodes");
				wrap.setAttribute("align", "left");
				but.setAttribute("onclick", 'Response("' + item.Id + '")');
				but.setAttribute("style",
					"height=25px; width:130px; display:inline-block; margin-left:0px; " +
					"margin-top:5px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");
				if(item.Avatar != "00000000-0000-0000-0000-000000000000")
				{
					var attachData = GetAttachData(item.Avatar);
					img.setAttribute("src", 'data:image/jpeg;base64,' + attachData.Data);
				}
				else
				{
					img.setAttribute("src", "./img/personWithoutImage.png");
				}
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
			PopUp("Ошибка получения пользователей: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

/// Message methods. ///

// Post method. //
// Отправляет запрос на создание сообщения. //
function SendMessage()
{
	var profileId = document.getElementById('profileId').value;
	var chatId = document.getElementById("chatId").value;
	var messageText = document.getElementById("messageArea").value;
	var preview = document.getElementById("preview");
	var timeToDestroy = document.getElementById("timer").value;
	document.getElementById("timer").value = 0;
	var attachId;
	if(messageText == "" && document.getElementById('file-input').files.length <= 0)
	{
		PopUp("Ошибка отправки сообщения: " + "пустое сообщение.", 1, false);
		return;
	}
	if(document.getElementById('file-input').files.length > 0)
	{
		attachId = LoadAttach();
	}
	else
	{
		attachId = 0;
	}
	var request = new XMLHttpRequest();
	request.open('POST', messageUrl, false);
	var message = '{ "ProfileId" : "' + profileId + '", "ChatId" : "' + chatId +
					'", "MessageText" : "' + messageText + '", "Attachment" : "' + attachId + '", "TimeToDestroy" : "' + timeToDestroy + '" } ';
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(message);
	}
	catch(Exception)
	{
		PopUp("Ошибка отправки сообщения: нет подключения к серверу", 1, false);
		return;
	}

	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			document.getElementById("messageArea").value = "";
			DeleteAttach();
			GetMessages(chatId);
			break;
		}
		default:
		{
			PopUp("Ошибка отправки: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Get method. //
// Отправляет запрос на получение последнего сообщения чата. //
function GetLastMessage(chatId)
{
	var profileId = document.getElementById("profileId").value;
	var request = new XMLHttpRequest();
	var url = messageUrl + "/chat/" + chatId + "/profile/" + profileId + "/noread";
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения сообщения: нет подключения к серверу", 1, false);
		return null;
	}
	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;
			var data = JSON.parse(responseBody); 
			return data;
		}
		default:
		{
			PopUp("Ошибка получения сообщения: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
	return null;
}

// Get method. //
// Отправляет запрос на получение всех сообщений чата. //
function GetMessages(chatId)
{
	DisableDiv(document.getElementById("chatsPanel"), 200, "px", 100, "vh");
	document.getElementById("chatId").value = chatId;
	var profileId = document.getElementById("profileId").value;
	ChatProfiles();
	var request = new XMLHttpRequest();
	var url = messageUrl + "/chat/" + chatId + "/profile/" + profileId;
	request.open('GET', url, true);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения сообщения: нет подключения к серверу", 1, false);
		EnableDiv(document.getElementById("chatsPanel"));
		return;
	}
	request.onreadystatechange = function()
	{
		if(request.readyState == 4)
		{
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
						var attach = document.createElement("img");
						var read = document.createElement("img");
						var profileId = document.getElementById("profileId").value;
						var profile = GetProfile(item.ProfileId);
						inf.innerHTML = profile.Name + " " + profile.Surname;
						if(profile.Login == "")
							inf.innerHTML += " ( *Профиль удалён* )";
						text.innerHTML = item.MessageText;
		
						text.setAttribute("class", "dialogMessage");
						inf.setAttribute("class", "messageInf");
						if(profileId == profile.Id)
						{
							text.setAttribute("style",
								"float:right;display:block;width:95%; margin-bottom:5px; margin-right:5px;word-wrap: break-word;");
							inf.setAttribute("style",
								"float:right;display:block;width:95%; margin-top:5px;  margin-right:5px;color: #4169E1");
							var regexp = /@(\d|\w)+/;
							var names = regexp.exec(item.MessageText);
							var gotcha = false;
							if(names != null)
								for(var j = 0; j < names.length; j++)
								{
									var logins = GetLogin(names[j].replace('@', ""));
									if(logins != null)
										if(logins.Id == profileId)
										{
											message.setAttribute("style",
											"float:right;margin-right:2%;display:block; width:50%;" +
											" background-color: #ffffff; -moz-border-radius: 10px;-webkit-border-radius: 9px; align-content: center; margin-top:10px;");	
											gotcha = true;
										}
								}
							if(!gotcha)
							{
								message.setAttribute("style",
									"float:right;margin-right:2%;display:block; width:50%;" + 
									" background-color: #0D0B15; -moz-border-radius: 10px;-webkit-border-radius: 9px; align-content: center; margin-top:10px;");
							}
						}
						else
						{
							text.setAttribute("style",
								"float:left;display:block;width:95%; margin-bottom:5px; margin-left:5px;word-wrap: break-word;");
							inf.setAttribute("style",
								"float:left;display:block;width:95%; margin-top:5px;  margin-left:5px; color:#FFBE33");
							var regexp = /@(\d|\w)+/;
							var gotcha = false;
							var names = regexp.exec(item.MessageText);
							if(names != null)
								for(var j = 0; j < names.length; j++)
								{
									var logins = GetLogin(names[j].replace('@', ""));
									if(logins != null)
										if(logins.Id == profileId)
										{
											message.setAttribute("style",
											"float:left;margin-left:2%;display:block; width:50%;" + 
											"background-color: #ffffff; -moz-border-radius: 10px;-webkit-border-radius: 9px; align-content: center; margin-top:10px;");	
											gotcha = true;
										}
								}
							if(!gotcha)
							{
								message.setAttribute("style",
									"float:left;margin-left:2%;display:block; width:50%;" + 
									"background-color: #1F1D10; -moz-border-radius: 10px;-webkit-border-radius: 9px; align-content: center; margin-top:10px;");
						
							}
						}
						message.appendChild(inf);
						if(item.Attachment == '00000000-0000-0000-0000-000000000001')
						{
							var deleted = document.createElement("div");
							deleted.innerHTML = "Сообщение удалено."
							deleted.setAttribute("style",
								"padding-left:10px;color:white;float:left;margin-left:2%;display:block; width:50%; background-color: #1F1D10; -moz-border-radius: 10px;-webkit-border-radius: 9px; align-content: center; margin-top:10px; margin-bottom:10px");
							message.appendChild(deleted);
						}
						else
							message.appendChild(text);
						if(item.Attachment != '00000000-0000-0000-0000-000000000000' && item.Attachment != '00000000-0000-0000-0000-000000000001')
						{
							var attachData = GetAttachData(item.Attachment);
							attach.setAttribute("src", 'data:image/png;base64,' + attachData.Data);
							attach.setAttribute("class", "image");
							attach.setAttribute("vspace", "15px");
							attach.setAttribute("hspace", "25px");
							message.appendChild(attach);
						}
						if(item.IsRead && item.ProfileId == profileId)
						{
							read.setAttribute("style","position:absolute;display:block-inline;");
							read.setAttribute("src", "./img/messageRead.png");
							read.setAttribute("class", "image");
							read.setAttribute("height", "12px");
							read.setAttribute("width", "21px");
							read.setAttribute("hspace", "10px");
							read.setAttribute("vspace", "5px");
							inf.appendChild(read);
						}
						if(item.TimeToDestroy > 0)
						{
							SelfDestroy(item.TimeToDestroy, chatId);
							inf.innerHTML += " Время до удаления: " + item.TimeToDestroy + " сек.";
						}
						mainList.appendChild(message);
					}
					document.getElementById("messageBox").setAttribute("style",
						"visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:39px; position:absolute; bottom:5px; left: 0px; min-width:200px");
					document.getElementById("dialogMeta").setAttribute("style",
						"visibility:visible; position:absolute;height:35px;width:100%;top:0;background-color:#C0C0C0;padding-left:15px;padding-top:0px;bottom:10px");
					var chatData = GetChat(chatId);
					document.getElementById("dialogName").innerHTML = chatData.ChatName;
					document.getElementById("dialogName").setAttribute("style",
					"visibility:visible; position:absolute;height:35px;width:100%;top:0;padding-left:40px;margin-top:0px;bottom:10px");
					document.getElementById("dialog").setAttribute("style",
						"visibility:visible; text-align:left; margin-bottom:25px; padding:0; background:#202020; height:calc(100vh - 150px); width:(100% - 20px); position: absolute; top:35px; overflow-y:auto;");
					document.getElementById("dialog").scrollTop = document.getElementById("dialog").scrollHeight;
					break;
				}
				default:
				{
					PopUp("Ошибка получения сообщений: " + request.status + ': ' + request.statusText, 1, false);
					break;
				}
			}
			EnableDiv(document.getElementById("chatsPanel"));
		}
	}
}

// Get method. //
// Отправляет запрос на подсчет сообщений в текущем чате. //
function UpdateMessages(chatId)
{
	var request = new XMLHttpRequest();
	//var chatId = document.getElementById("chatId").value;
	var profileId = document.getElementById("profileId").value;
	if(chatId == "")
		return;
	var url = messageUrl + "/chat/" + chatId + "/profile/" + profileId + "/count";
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка подсчета: нет подключения к серверу", 1, false);
		return false;
	}

	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			if(data != document.getElementById("count" + chatId).value)
			{
				document.getElementById("count" + chatId).value = data;
				return true
				//GetMessages(chatId);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка подсчета: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
	return false;
}

// Get method. //
// Отправляет запрос на подсчет прочитанных сообщений в текущем чате. //
function CountReadMessages(chatId)
{
	var request = new XMLHttpRequest();
	//var chatId = document.getElementById("chatId").value;
	var profileId = document.getElementById("profileId").value;
	if(chatId == "")
		return;
	var url = messageUrl + "/chat/" + chatId + "/profile/" + profileId + "/read";
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка подсчета прочитанных: нет подключения к серверу", 1, false);
		return false;
	}

	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			if(data != document.getElementById("read" + chatId).value)
			{
				document.getElementById("read" + chatId).value = data;
				return true;
				//GetMessages(chatId);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка подсчета прочитанных: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
	return false;
}

// Get method. //
// Отправляет три запроса: на поиск среди профилей, среди чатов и среди сообщений. //
function Find()
{
	var request = new XMLHttpRequest();
	var string = document.getElementById("findString").value;
	var results = document.getElementById("searchResults");
	results.setAttribute("style", "color:white");
	results .innerHTML = "";
	var reg = /\s*,\s*/;
	var array = string.split(reg);
	var url = profileUrl + "/find/profiles";
	if(array.length < 1)
	{
		PopUp("Ошибка поиска: пустой запрос", 1, false);
		return;
	}
	var names = '{ "tokens" : [ "' + array[0] + '"';
	for(var i = 1; i < array.length; i++)
		names += ', "' + array[i] + '" ';
	names += ' ], "profileId" : "' + document.getElementById("profileId").value + '" }';
	request.open('POST', url, false);
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(names);
	}
	catch(Exception)
	{
		PopUp("Ошибка поиска по пользователям: нет подключения к серверу", 1, false);
		return;
	}

	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			if(data.length <= 0)
			{
				results .innerHTML = "<br/>Профилей не найдено<br/>";
			}
			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var wrap = document.createElement("div");
				var but = document.createElement("label");
				var button = document.createElement("div");
				var img = document.createElement("img");
				var addImg = document.createElement("img");

				but.innerHTML = item.Name + " " + item.Surname;
				wrap.setAttribute("class", "chatNodes");
				wrap.setAttribute("align", "left");
				wrap.setAttribute("title", item.Name + " " + item.Surname);
				but.setAttribute("onclick", 'AddToChat("' + item.Id + '")');
				but.setAttribute("style",
					"height=25px; width:110px; display:inline-block; margin-left:30px; " +
					"margin-top:5px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");

				button.setAttribute("style", "height=25px; width=25px;right:10px;position:absolute; float:right; background-color:#808080");
				addImg.src = "./img/addperson.png";
				img.setAttribute("style", "display:inline-block;left:15px; position:absolute");
				img.setAttribute("height", "25px");
				img.setAttribute("width", "25px");
				img.setAttribute("vspace", "5px");
				img.setAttribute("hspace", "5px");
				addImg.setAttribute("style", "display:inline-block; right:5px; position:absolute");
				addImg.setAttribute("height", "25px");
				addImg.setAttribute("width", "25px");
				addImg.setAttribute("vspace", "5px");
				addImg.setAttribute("hspace", "5px");
				if (item.Avatar != "00000000-0000-0000-0000-000000000000")
				{
					var attachData = GetAttachData(item.Avatar);
					img.setAttribute("src", 'data:image/jpeg;base64,' + attachData.Data);
				}
				else
				{
					img.setAttribute("src", "./img/personWithoutImage.png");
				}

				wrap.appendChild(img);
				wrap.appendChild(but);
				wrap.appendChild(button);
				wrap.appendChild(addImg);
				results.appendChild(wrap);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка поиска по пользователям: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}

	url = chatUrl + "/find/chats";
	request.open('POST', url, false);
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(names);
	}
	catch(Exception)
	{
		PopUp("Ошибка поиска по чатам: нет подключения к серверу", 1, false);
		return;
	}
	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			if(data.length <= 0)
			{
				results .innerHTML += "<br/>Чатов не найдено<br/>";
			}
			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var wrap = document.createElement("div");
				var but = document.createElement("label");
				var button = document.createElement("div");
				var img = document.createElement("img");

				but.innerHTML = item.ChatName;
				wrap.setAttribute("class", "chatNodes");
				wrap.setAttribute("align", "left");
				but.setAttribute("onclick", 'GetMessages("' + item.ChatId + '")');
				but.setAttribute("style",
					"height=25px; width:110px; display:inline-block; margin-left:30px; margin-top:5px; " +
					"position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");

				button.setAttribute("style", "height=25px; width=25px;right:10px;position:absolute; float:right; background-color:#808080");
				img.setAttribute("style", "display:inline-block;left:15px; position:absolute");
				img.setAttribute("height", "25px");
				img.setAttribute("width", "25px");
				img.setAttribute("vspace", "5px");
				img.setAttribute("hspace", "5px");
				img.setAttribute("src", "./img/chatWithoutImage.png");
				wrap.appendChild(img);
				wrap.appendChild(but);
				wrap.appendChild(button);
				results.appendChild(wrap);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка поиска по чатам: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}

	url = messageUrl + "/find/messages";
	request.open('POST', url, false);
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(names);
	}
	catch(Exception)
	{
		PopUp("Ошибка поиска по чатам: нет подключения к серверу", 1, false);
		return;
	}
	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			if(data.length <= 0)
			{
				results .innerHTML += "<br/>Сообщений не найдено<br/>";
			}
			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var wrap = document.createElement("div");
				var but = document.createElement("label");
				var author = document.createElement("label");
				var button = document.createElement("div");
				var img = document.createElement("img");
				var profile = GetProfile(item.ProfileId);
				var chat = GetChat(item.ChatId);

				but.innerHTML = item.MessageText + "<br/>";
				author.innerHTML = profile.Name + " " + profile.Surname + " из " + chat.ChatName;
				wrap.setAttribute("class", "personeNodes");
				wrap.setAttribute("align", "left");
				wrap.setAttribute("title", item.MessageText + " от " + profile.Name + " " + profile.Surname + " из " + chat.ChatName + " отправлено от " + item.Date);
				but.setAttribute("onclick", 'GetMessage("' + item.MessageId + '")');
				but.setAttribute("style",
					"height=30px; width:110px; display:inline-block; margin-left:30px; margin-top:1px; " +
					"position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");
				author.setAttribute("style",
					"height=30px; width:150px; display:inline-block; margin-left:15%; margin-top:17px; " +
					"position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap; font-size:80%");

				button.setAttribute("style", "height=25px; width=25px;right:10px;position:absolute; float:right; background-color:#808080");
				img.setAttribute("style", "display:inline-block;left:15px; position:absolute");
				img.setAttribute("height", "25px");
				img.setAttribute("width", "25px");
				img.setAttribute("vspace", "5px");
				img.setAttribute("hspace", "5px");
				img.setAttribute("src", "./img/message.png");
				wrap.appendChild(img);
				wrap.appendChild(but);
				wrap.appendChild(author);
				wrap.appendChild(button);
				results.appendChild(wrap);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка поиска по сообщениям: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

/// Attachment methods. ///

// Post method. //
// Загружает картинку в базу данных. //
function LoadAvatar()
{
	var img = document.getElementById("avatar");
	if(document.getElementById("avatarFile").value == "")
	{
		CreateUser(0);
	}
	var request = new XMLHttpRequest();
	var data = '{ "AttachId" : "0", "Data" : "' + getBase64Image(img, 150, 200) + '", "Type" : "' + document.getElementById("avatarFile").files[0].type + '" }';
	request.open('POST', attachmentUrl, false);
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(data);
	}
	catch(Exception)
	{
		PopUp("Ошибка отправки файла: нет подключения к серверу", 1, false);
		return;
	}
	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			CreateUser(data.AttachId);
			break;
		}
		default:
		{
			PopUp("Ошибка отправки файла: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Post method. //
// Загружает картинку в базу данных. //
function LoadAttach()
{
	var img = document.getElementById("previewImage");
	var request = new XMLHttpRequest();
	var data = '{ "AttachId" : "0", "Data" : "' + getBase64Image(img, 100, 100) + '", "Type" : "' + document.getElementById("file-input").files[0].type + '" }';
	request.open('POST', attachmentUrl, false);
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(data);
	}
	catch(Exception)
	{
		PopUp("Ошибка отправки файла: нет подключения к серверу", 1, false);
		return;
	}
	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			PopUp("Вложение загружено.", 0, true);
			return data.AttachId;
		}
		default:
		{
			PopUp("Ошибка отправки файла: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Get method. //
// Отправляет запрос на получение файла по его ИД. //
function GetAttachData(id)
{
	var request = new XMLHttpRequest();
	var url = attachmentUrl + "/" + id;
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения вложения: нет подключения к серверу", 1, false);
		return;
	}

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
			PopUp("Ошибка получения вложения: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}
