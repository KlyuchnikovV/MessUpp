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
	//var avatar = getBase64Image(document.getElementById("avatarFile"));

	var request = new XMLHttpRequest();

	// Может понадобиться сменить адрес, добавить аватар и асинхронность. //

	request.open('POST', profileUrl, false);
	var user = '{ "Login" : "' + login + '", "Password" : "' + password + '", "Name" : "' + name + '", "Surname" : "' + surname + '", "Avatar" : [' + 0 + '] }';
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

	if (request.status != 200)
	{
		PopUp("Ошибка создания юзера: " + request.status + ': ' + request.statusText, 1, false);
	}
	else
	{
		responseBody = request.responseText;
		var data = JSON.parse(responseBody);
		console.log(data);
	}
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
			console.log(data);
			window.location = "main.html?id=" + data.Id;
			document.getElemehintById("profileId").value = data.Id;
			break;
		}
		default:
		{
			PopUp("Ошибка входа: " + request.status + ': ' + request.statusText, 1, false);			
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
				but.setAttribute("onclick", 'GetMessages("' + item.ChatId + '")');
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
			PopUp("Ошибка получения юзера: " + request.status + ': ' + request.statusText, 1, false);
			
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
			break;
		}
		default:
		{
			PopUp("Ошибка создания чата: " + request.status + ': ' + request.statusText, 1, false);			
			
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
			GetMessages(chatId);
			document.getElementById("messageArea").value = "";
			// Update dialog, clean message box, add attachments
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
				var profileId = document.getElementById("profileId").value;
				var profile = GetProfile(item.ProfileId);
				array = item.MessageText.split(/\s*(\W|\D)+\s*/);
				//alert(array);
				for(var j = 0; j < array.length; j++)
				{
					if(array[j] != "")
						cache.value += array[j] + "$";
				}
				inf.innerHTML = profile.Name + " " + profile.Surname;
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
				mainList.appendChild(message);

				var words = item.MessageText.split(" ");
				cache.value = cache.value + " " + words[0];

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

// Get method. //
function Find()
{
	var request = new XMLHttpRequest();
	var string = document.getElementById("findString").value;
	var results = document.getElementById("searchResults");
	results .innerHTML = "";
	var reg = /\s*,\s*/;
	var array = string.split(reg);
	var url = profileUrl + "/profiles";
	if(array.length < 1)
	{
		PopUp("Ошибка поиска: пустой запрос", 1, false);			
		return;
	}
	var names = '{ "names" : [ "' + array[0] + '"';
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
					"height=25px; width:110px; display:inline-block; margin-left:30px; margin-top:5px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");

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

				// Add pictures
				img.setAttribute("src", "./img/personWithoutImage.png");

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
			PopUp("Ошибка поиска: " + request.status + ': ' + request.statusText, 1, false);			
			break;
		}
	}


	url = chatUrl + "/chats";

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
					"height=25px; width:110px; display:inline-block; margin-left:30px; margin-top:5px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");

				button.setAttribute("style", "height=25px; width=25px;right:10px;position:absolute; float:right; background-color:#808080");
				img.setAttribute("style", "display:inline-block;left:15px; position:absolute");
				img.setAttribute("height", "25px");
				img.setAttribute("width", "25px");
				img.setAttribute("vspace", "5px");
				img.setAttribute("hspace", "5px");

				// Add pictures
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
			PopUp("Ошибка поиска: " + request.status + ': ' + request.statusText, 1, false);			
			break;
		}
	}

	url = messageUrl + "/messages";
	
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
					"height=30px; width:110px; display:inline-block; margin-left:30px; margin-top:1px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");
				author.setAttribute("style", 
					"height=30px; width:150px; display:inline-block; margin-left:15%; margin-top:17px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap; font-size:80%");

				button.setAttribute("style", "height=25px; width=25px;right:10px;position:absolute; float:right; background-color:#808080");
				img.setAttribute("style", "display:inline-block;left:15px; position:absolute");
				img.setAttribute("height", "25px");
				img.setAttribute("width", "25px");
				img.setAttribute("vspace", "5px");
				img.setAttribute("hspace", "5px");

				// Add pictures
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
			PopUp("Ошибка поиска: " + request.status + ': ' + request.statusText, 1, false);			
			break;
		}
	}
}

// Get method. //
function AddToChat(id)
{
	var request = new XMLHttpRequest();
	var chatId = document.getElementById("chatId").value;
	if(chatId == "")
	{
		PopUp("Ошибка добавления к чату: не выбранн чат", 1, false);			
		return;
	}
	var url = chatUrl + "/" + chatId + "/" + id;
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
			PopUp("Успешно!", 0, true);
			break;
		}
		default:
		{
			PopUp("Ошибка добавления: " + request.status + ': ' + request.statusText, 1, false);			
			break;
		}
	}
}

// Get method. //
function UpdateMessages()
{
	var request = new XMLHttpRequest();
	var chatId = document.getElementById("chatId").value;
	if(chatId == "")
		return;
	var url = messageUrl + "/count/" + chatId;
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
			var responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			if(data != document.getElementById("count").value)
			{
				GetMessages(chatId);
			}
			document.getElementById("count").value = data;
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
function ChatProfiles()
{
	var id = document.getElementById('chatId').value;
	var request = new XMLHttpRequest();
	var url = chatUrl + "/" + id + "/profiles";
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
				wrap.setAttribute("class", "personeNodes");
				wrap.setAttribute("align", "left");
				but.setAttribute("onclick", 'Response("' + item.Id + '")');
				but.setAttribute("style", 
					"height=25px; width:130px; display:inline-block; margin-left:0px; margin-top:5px; position:absolute;text-overflow: ellipsis;padding: 5px;overflow: hidden;white-space: nowrap;");
				
				// If no pic then use default


				// Add pictures
				img.setAttribute("src", "./img/personWithoutImage.png");

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