$(function () {
    $('#loginBlock').show();
    var game = $.connection.gameHub;

    game.client.onAlreadyPresent = function (name) {
        var message = 'Competitor with name <span class="bold">' + name + '</span> already present. Please input other name.';
        showMessage(message);
    }

    game.client.onMaxCount = function (count) {
        var message = 'Number of competitors reached a maximum value <span class="bold">(' + count + ')</span>. Try to join later.';
        showMessage(message);
    }

    game.client.onIncorrect = function (answer) {
        var message = '<span class="bold">' + answer + '</span> is incorrect answer... Try again!';
        showMessage(message);
    }

    game.client.onRoundFinish = function (name) {
        var message = 'Competitor <span class="bold">' + name + '</span> won in this round! He got 1 point!';
        showMessage(message);
        $("#wait").show();
        $("#question").hide();
    }

    game.client.onUserConnected = function (userName, allUsers) {
        $("#loginBlock").hide();
        $("#compitForm").show();
        $("#username").val(userName);
        $("#currentUser").html(userName);
        UpdateUsers(allUsers);       
    }

    game.client.onUserDisconnected = function (id) {
        $('#' + id).remove();
    }

    game.client.onUpdateUsers = function (allUsers) {
        UpdateUsers(allUsers);
    }

    game.client.onNewQuestion = function (qstnId, qstnText, answers) {
        $("#wait").hide();
        $("#question").show();
        $(":radio").prop("checked", false);
        $("#qstnId").val(qstnId);
        $("#qstnText").html(qstnText);
        var $variants = $(".variant");
        for (var i = 0; i < $variants.length; i++) {
            $(":radio", $variants[i]).val(answers[i]);
            $("span", $variants[i]).html(answers[i]);
        }
    }

    $.connection.hub.start().done(function () {
        $('#submit').click(function () {
            var $selected = $(":radio:checked");
            if ($selected.length === 0) return;
            game.server.sendResponse($("#qstnId").val(), $selected.val());
        });

        $("#btnLogin").click(function () {
            var name = $("#txtUserName").val();
            if (name.length > 0)
                game.server.connect(name);
            else
                showMessage("Input your name!");
        });
    });
});

function showMessage(message) {
    $("#errorName").html(message);
    $("#errorModal").modal("show");
    setTimeout(function () { $("#errorModal").modal("hide"); }, 3000);
}

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function UpdateUsers(allUsers) {
    $("#result").html("");
    for (i = 0; i < allUsers.length; i++) {
        var user = allUsers[i];
        $("#result").append('<p id="' + user.ConnectionId + '"><span class="bold">' + user.Name + " - " + user.Points + '</span></p>');
    }
}