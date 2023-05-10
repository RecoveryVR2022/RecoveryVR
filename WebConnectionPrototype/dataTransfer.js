let socket = new WebSocket("ws://192.168.1.104:3000");

socket.onopen = function(e) {
  alert("[open] Connection established");
  //alert("Sending to server");
  //socket.send("My name is John");
};

socket.onmessage = function(event) {
  alert(`[message] Data received from server: ${event.data}`);
};

socket.onclose = function(event) {
  if (event.wasClean) {
    alert(`[close] Connection closed cleanly, code=${event.code} reason=${event.reason}`);
  } else {
    // e.g. server process killed or network down
    // event.code is usually 1006 in this case
    alert('[close] Connection died');
  }
};

socket.onerror = function(error) {
  alert(`[error]`);
};

function transferData(data) {
    var jsonString = JSON.stringify(data);
    console.log('Accepted：' + jsonString);

    // networking part
socket.send(jsonString);
//var xhr = new XMLHttpRequest();
//      xhr.open("POST", "http://192.168.1.104:8080", true);
//      xhr.setRequestHeader("Content-Type", "text/plain");
//      xhr.send(jsonString);
}