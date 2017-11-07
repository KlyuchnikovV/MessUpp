
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
	//alert(user);
	request.setRequestHeader("Content-type", "application/json");
	request.send(user);

	if (request.status != 200)
	{
		alert( request.status + ': ' + request.statusText );
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
			alert("Добро пожаловать, " + data.Name + " " + data.Surname + "!");
			window.location = "main.html?id=" + data.Id;
			//alert("main.html?id=" + data.Id);
			document.getElementById("profileId").value = data.Id;
			break;
		}
		default:
		{
			alert( request.status + ': ' + request.statusText );
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
	//alert(url);
	request.open('GET', url, false);
	request.send(null);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			var mainList = document.getElementById("chatList");

			for(var i = 0; i < data.length; i++)
			{
				var item = data[i];
				var but = document.createElement("div");
				but.innerHTML = item.ChatName;
				but.setAttribute("class", "chatNodes");
				but.setAttribute("style", "border:1px solid #CD853F; float:left; height:10%; width: 90%; margin-left:5%; margin-top:2%");
				but.setAttribute("onclick", "GetMessages()");
				mainList.appendChild(but);
			}
			break;
		}
		default:
		{
			alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}

// Get method. //
function GetProfile(id)
{
	var request = new XMLHttpRequest();
	var url = profileUrl + "/" + id;
	//alert(url);
	request.open('GET', url, false);
	request.send(null);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			return data;
			//break;
		}
		default:
		{
			alert( request.status + ': ' + request.statusText );
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
	//alert(chat);
	request.setRequestHeader("Content-type", "application/json");
	request.send(chat);


	switch(request.status)
	{
		case 200:
		{
			responseBody = request.responseText;
			var data = JSON.parse(responseBody);
			alert("Чат " + chatName + " создан! ");
			//window.location = "main.html?id=" + data.Id;
			//document.getElementById("profileId").value = data.Id;
			GetChats(id);
			break;
		}
		default:
		{
			alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}

// Post method. //
function SendMessage()
{
	var profileId = document.getElementById('profileId').value;
	var chatId = document.getElementById("chatId").value;
	//alert("");
	var messageText = document.getElementById("messageArea").value;
	//alert(messageText);
	if(messageText == "")
	{
		alert("Пустое сообщение");
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
			alert("Сообщение отправлено.");
			GetMessages();
			// Update dialog, clean message box, add attachments
			break;
		}
		default:
		{
			alert( request.status + ': ' + request.statusText );
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
				var wrap = document.createElement("div");
				var but = document.createElement("div");
				var profile = GetProfile(item.ProfileId);
				wrap.innerHTML = profile.Name + " " + profile.Surname;
				but.innerHTML = item.MessageText;
				but.setAttribute("class", "dialogMessage");
				wrap.setAttribute("class", "messageInf");
				if(item.ProfileId == profile.Id)
				{
					but.setAttribute("style", "float:right;margin-left:49%;");
					wrap.setAttribute("style", "float:right;margin-left:49%;");
				}
				else
				{
					but.setAttribute("style", "float:left;margin-left:1%;");
					wrap.setAttribute("style", "float:left;margin-left:1%;")
				}
				wrap.appendChild(but);
				mainList.appendChild(wrap);
			}
			//document.getElementById("messageBox").setAttribute("style", "visibility:visible; float:right;margin-left:49%; margin-bottom:5px");
			//document.getElementById("attachButton").setAttribute("style", "visibility:visible; margin-bottom:5px; margin-left:10px");
			//document.getElementById("messageInput").setAttribute("style", "visibility:visible; margin-bottom:5px; width:60%;margin-right:10px");
			//document.getElementById("messageButton").setAttribute("style", "visibility:visible; margin-bottom:5px; margin-right:10px");
			break;
		}
		default:
		{
			alert( request.status + ': ' + request.statusText );
			break;
		}
	}
}
