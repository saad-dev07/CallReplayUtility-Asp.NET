var SignalrConnection = $.hubConnection();
_Proxy = SignalrConnection.createHubProxy('MainHub');   
    RefreshCallsReport();
    _Proxy.on("recieveCallsData", function () {
        $('#myModal').show();
      //  alert('test');
        initTable();

      //  fnClickAddRow();
       $('#myModal').hide();
    });



function RefreshCallsReport() {
    SignalrConnection.start().done(function () {
        _Proxy.invoke('SendCallsRefreshNotification');
    })
        .fail(function () {
            //  alert("failed in connecting to the signalr server2");
        })
}