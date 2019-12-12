# VRKnockServer

Server for [VRKnockApp](https://github.com/InventivetalentDev/VRKnockApp) which utilizes the OpenVR Notification API to display notifications in Virtual Reality triggered by the mobile app.


## API
The server works by running a local web server on port `16945` which exposes the following endpoints:

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


### `POST /status`
Can be used to check if the server is running and if VR is active.  
Body: 
```json
{"code":"<code given by the server info window>"}
```


### `POST /triggerKnock`
Used to trigger a knock.  
Body:
```json
{
  "code":"<code given by the server info window>",
  "message":"<message to show>"
}
```
