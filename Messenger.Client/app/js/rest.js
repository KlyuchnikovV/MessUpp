const fs = require('fs');

// Пути к контроллерам
var profileUrl = "http://localhost:49856/api/profile";
var chatUrl = "http://localhost:49856/api/chat";
var messageUrl = "http://localhost:49856/api/message";
var attachmentUrl = "http://localhost:49856/api/attach";

/// Profile methods. ///

// Post method. //
//  Отправляет запрос на создание нового профиля. //
function CreateUser(id)
{
	var name = document.getElementById("txtName").value;
	var surname = document.getElementById("txtSurname").value;
	var login = document.getElementById("txtLogin").value;
	var password = document.getElementById("txtPassword").value;
	var request = new XMLHttpRequest();
	request.open('POST', profileUrl, false);
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
		return;
	}

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
}

// Get method. //
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

// Delete method. //
// Отправляет запрос на удаление профиля по ИД. //
function DeleteProfile()
{
	var request = new XMLHttpRequest();
	var id = document.getElementById("profileId").value;
	var url = profileUrl + "/" + id;
	request.open('DELETE', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка удаления профиля: нет подключения к серверу", 1, false);
		return;
	}

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
}

// Get method. //
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

// Post method. //
// Отправляет запрос на поиск профиля по логину и паролю. //
function Login()
{
    var login = document.getElementById("txtLogin").value;
    var password = document.getElementById("txtPassword").value;
	var request = new XMLHttpRequest();
	request.open('POST', profileUrl + "/login", false);
	var user = '{ "Login" : "' + login + '", "Password" : "' + password + '" }';
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(user);
	}
	catch(Exception)
	{
		PopUp("Ошибка входа: нет подключения к серверу", 1, false);
		return;
	}
	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			window.location = "main.html?id=" + data.Id;
			document.getElemehintById("profileId").value = data.Id;
			break;
		}
		case 203:
		{
			PopUp("Ошибка входа: данная пара логин/пароль не существует.", 1, false);
			break;
		}
		default:
		{
			PopUp("Ошибка входа: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

/// Chat methods. ///

// Post method. //
// Отправляет запрос на создание чата. //
function CreateChat()
{
	var id = document.getElementById('profileId').value;
	var chatName = document.getElementById("txtChatName").value;
	var request = new XMLHttpRequest();
	request.open('POST', chatUrl, false);
	var chat = '{ "ChatName" : "' + chatName + '", "ChatMembers" : [ { "Id" : "' + id + '" } ] }';
	request.setRequestHeader("Content-type", "application/json");
	try
	{
		request.send(chat);
	}
	catch(Exception)
	{
		PopUp("Ошибка создания чата: нет подключения к серверу", 1, false);
		return;
	}
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
}

// Get method. //
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

// Delete method. //
// Отправляет запрос на удаление чата по ИД. //
function DeleteChat(id)
{
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
		return;
	}

	switch(request.status)
	{
		case 200:
		case 204:
		{
			PopUp("Чат успешно удален.", 0, true);
			GetChats();
			return;
		}
		default:
		{
			PopUp("Ошибка удаления чата: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Get method. //
// Отправляет запрос на добавление выбранного пользователя к текущему чату. //
function AddToChat(id)
{
	var request = new XMLHttpRequest();
	var chatId = document.getElementById("chatId").value;
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

// Delete method. //
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
			"visibility:hidden;background-color:#202020; vertical-align:center; width:100%; height:39px; position:absolute; bottom:5px; left: 0px; min-width:765px");
			document.getElementById("dialogMeta").setAttribute("style",
				"visibility:hidden; position:absolute;height:35px;width:100%;top:0;background-color:#C0C0C0;padding-left:15px;padding-top:0px;bottom:10px");
			document.getElementById("dialogName").setAttribute("style",
			"visibility:hidden; position:absolute;height:35px;width:100%;top:0;padding-left:15px;margin-top:0px;bottom:10px");
			document.getElementById("dialog").setAttribute("style",
				"visibility:hidden; text-align:left; margin-bottom:25px; padding:0; background:#202020; height:calc(100vh - 100px); width:(100% - 20px); position: absolute; top:35px; overflow-y:auto;");
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

// Get method. //
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
				but.innerHTML = item.Name + " " + item.Surname;
				wrap.setAttribute("title", item.Name + " " + item.Surname);
				wrap.setAttribute("class", "personeNodes");
				wrap.setAttribute("align", "left");
				but.setAttribute("onclick", 'Response("' + item.Id + '")');
				but.setAttribute("style", 
					"height=25px; width:130px; display:inline-block; margin-left:0px; " + 
					"margin-top:5px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");
				if(item.Avatar != null)
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
			PopUp("Ошибка получения чатов: " + request.status + ': ' + request.statusText, 1, false);			
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
			preview.innerHTML = "";
			document.getElementById('file-input').innerHTML = document.getElementById('file-input').innerHTML;
			document.getElementById('messageBox').setAttribute("style",
			"visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:39px; position:absolute; bottom:5px; left: 0px; min-width:765px");   
			GetMessages(chatId);
			SelfDestroy(timeToDestroy, chatId);
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
// Отправляет запрос на получение всех сообщений чата. //
function GetMessages(chatId)
{
	/*var Textcomplete = require('../../node_modules/textcomplete/lib/textcomplete');
	var Textarea = require('../../node_modules/textcomplete/lib/textarea');

	var editor = new Textarea(textareaElement);
	var textcomplete = new Textcomplete(editor);

	var textareaElement = document.getElementById('messageArea')
	var editor = new Textarea(elementElement);
	
	var textcomplete = new Textcomplete(editor, 
		{
	  dropdown: {
		maxCount: Infinity
	  }
	});

	textcomplete.register([{
		// Emoji strategy
		match: /(^|\s):(\w+)$/,
		search: function (term, callback) {
		  callback(emojies.filter(emoji => { return emoji.startsWith(term); }));
		},
		replace: function (value) {
		  return '$1:' + value + ': ';
		}
	  }]);*/

	document.getElementById("chatId").value = chatId;
	ChatProfiles();
	var request = new XMLHttpRequest();
	var url = messageUrl + "/chat/" + chatId;
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка получения сообщения: нет подключения к серверу", 1, false);
		return;
	}
	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;
			var cache = document.getElementById("cache");
			var mainList = document.getElementById("dialog");
			mainList.innerHTML = "";
			var data = JSON.parse(responseBody);
			var array;
			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var inf = document.createElement("div");
				var text = document.createElement("div");
				var message = document.createElement("div");
				var attach = document.createElement("img");
				var profileId = document.getElementById("profileId").value;
				var profile = GetProfile(item.ProfileId);
				//array = item.MessageText.split(/\s*(\W|\D)+\s*/);
				//for(var j = 0; j < array.length; j++)
				//{
				//	if(array[j] != "")
				//		cache.value += array[j] + "$";
				//}
				inf.innerHTML = profile.Name + " " + profile.Surname;
				if(profile.Login == "")
					inf.innerHTML += " \\Профиль удалён\\";
				text.innerHTML = item.MessageText;
				text.setAttribute("class", "dialogMessage");
				inf.setAttribute("class", "messageInf");
				if(profileId == profile.Id)
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
				if(item.Attachment != '00000000-0000-0000-0000-000000000000')
				{
					var attachData = GetAttachData(item.Attachment);
					attach.setAttribute("src", 'data:image/png;base64,' + attachData.Data);
					attach.setAttribute("height", "200px");
					attach.setAttribute("width", "200px");
					attach.setAttribute("vspace", "15px");
					attach.setAttribute("hspace", "25px");
					message.appendChild(attach);
				}
				mainList.appendChild(message);
				//var words = item.MessageText.split(" ");
				//cache.value = cache.value + " " + words[0];
			}
			document.getElementById("messageBox").setAttribute("style",
				"visibility:visible;background-color:#202020; vertical-align:center; width:100%; height:39px; position:absolute; bottom:5px; left: 0px; min-width:765px");
			document.getElementById("dialogMeta").setAttribute("style",
				"visibility:visible; position:absolute;height:35px;width:100%;top:0;background-color:#C0C0C0;padding-left:15px;padding-top:0px;bottom:10px");
			var chatData = GetChat(chatId);
			document.getElementById("dialogName").innerHTML = chatData.ChatName;
			document.getElementById("dialogName").setAttribute("style",
			"visibility:visible; position:absolute;height:35px;width:100%;top:0;padding-left:40px;margin-top:0px;bottom:10px");
			document.getElementById("dialog").setAttribute("style",
				"visibility:visible; text-align:left; margin-bottom:25px; padding:0; background:#202020; height:calc(100vh - 100px); width:(100% - 20px); position: absolute; top:35px; overflow-y:auto;");
			document.getElementById("dialog").scrollTop = document.getElementById("dialog").scrollHeight;
			break;
		}
		default:
		{
			PopUp("Ошибка получения сообщений: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
}

// Get method. //
// Отправляет запрос на подсчет сообщений в текущем чате. //
function UpdateMessages()
{
	var request = new XMLHttpRequest();
	var chatId = document.getElementById("chatId").value;
	if(chatId == "")
		return;
	var url = messageUrl + "/chat/" + chatId + "/count";
	request.open('GET', url, false);
	try
	{
		request.send(null);
	}
	catch(Exception)
	{
		PopUp("Ошибка подсчета: нет подключения к серверу", 1, false);
		return;
	}

	switch(request.status)
	{
		case 200:
		{
			var responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			if(data != document.getElementById("count").value)
			{
				document.getElementById("count").value = data;
				GetMessages(chatId);
			}
			break;
		}
		default:
		{
			PopUp("Ошибка подсчета: " + request.status + ': ' + request.statusText, 1, false);
			break;
		}
	}
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
				if(item.Avatar != null)
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
				results .innerHTML += "<br/>Чатов не найдено<br/><br/>";
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
	if(!img)
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

function getBase64Image(img, width, height)
{
	var canvas = document.createElement("canvas");
	canvas.width = img.width;
	canvas.height = img.height;
	var ctx = canvas.getContext("2d");
	ctx.drawImage(img, 0, 0, width, height);
	var dataURL = canvas.toDataURL("image/png");
	return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
}

async function SelfDestroy(timeToUpdate, chatId)
{
	setTimeout(
		function()
		{
			GetMessages(chatId);
		}, timeToUpdate * 1000);
}

function LoadProfileInfo()
{
	var profileId = document.getElementById("profileId").value;

	var item = GetProfile(profileId);
	var results = document.getElementById("loginInfo");
	var wrap = document.createElement("div");
	var but = document.createElement("label");
	var button = document.createElement("div");
	var img = document.createElement("img");

	but.innerHTML = item.Name + " " + item.Surname + "<br/>" + item.Login;
	but.setAttribute("style", "display:inline-block;left:50px; position:absolute; color:white");
	wrap.setAttribute("align", "left");
	wrap.setAttribute("title", item.Name + " " + item.Surname);
	img.setAttribute("style", "display:inline-block;left:15px; position:absolute");
	img.setAttribute("height", "25px");
	img.setAttribute("width", "25px");
	img.setAttribute("vspace", "5px");
	img.setAttribute("hspace", "5px");
	if(item.Avatar != null)
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
	results.appendChild(wrap);
}