# EC-Schedule-RestAPI
 A project with the purpose of implementing a quick setup asp .net rest api for the EC Schedule system. The endgoal is to have the api be implemented with SignalR and websockets for live update refreshing.
 
 !Disclamer!
 For security purposes, the code implementation of the database connection is done through reading a local config file. To successfully clone this repository: Create a config file in the EC-Schedule-RESTAPI project, with the first line being a valid connection string to a MySQL database. Or change the way the data context is registered in the DI container.
