

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//var connection = new sig

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function encodeText(text) {
    if (text != undefined && text != null) {
        text = text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        const encodedHtml = $("</div>").text(text).html();
        return encodedHtml;
    }
    return "";
}

$("#sendmessage").click(function (event) {
    event.preventDefault();
    if ($("#message").val() === undefined || $("#message").val() === "") {
        alert("Type a message");
        return;
    }
    const message = {
        Content: $("#message").val(),
        RecipientName: $("#choose").val() === "User" ? $("#user option:selected").text() : $("#chatgroup option:selected").text()
    };
    if ($("#choose").val() === "User") {
        if (message.RecipientName === "All")
            connection.invoke("SendMessageAll", message);
        else
            connection.invoke("Send message to user", message);
    }
    else {
        connection.invoke("Send message to group", message);
    }
    $("#message").val("").focus();
});

connection.on("ShowMessage", function (message) {
    var li = message.content;
    $("#discussion").append("<li>" + message.author+' : '+message.content + "</li>")
   // $("#discussion li").append(li);
});