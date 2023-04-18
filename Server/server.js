const http = require('http');
const express = require('express');
const socketio = require('socket.io');
const cors = require('cors');

const app = express();
app.use(cors());

const server = http.createServer(app);
const io = socketio(server, {
  cors: {
    origin: '*'
  }
});

const port = 3000;

io.on('connection', (socket) => {
  console.log('New client connected');

  socket.on('message', (message) => {
    console.log('Received message:', message);
    io.emit('message', message);
  });

  socket.on('disconnect', () => {
    console.log('Client disconnected');
  });
});

server.listen(port, () => {
  console.log(`WebSocket server is listening on port ${port}`);
});

