# VRKnockServer

Server for [VRKnockApp](https://github.com/InventivetalentDev/VRKnockApp) which utilizes the OpenVR Notification API to display notifications in Virtual Reality triggered by the mobile app.


## API
The server works by running a local web socket server on port `16945` which exposes the following endpoints:

All endpoints return a status:
```json
{
  "code": 0,
  "msg": "All good!"
}
```
```json
{
  "code": 1,
  "msg": "Something went wrong :("
}
```

