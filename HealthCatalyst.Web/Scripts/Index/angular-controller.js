
var host_uri = window.location.origin;
var user_uri = '/Home/User';
var app_uri = host_uri + user_uri;

app.controller('userCtrl', function ($scope, $http, UserService, Upload) {

    $scope.userData = null;
    $scope.searchVal = '';

    //Get all Users....
    UserService.GetAllUsers().then(function (response) {
        if (response.data.d != null && response.data.StatusCode == 200 && response.data.StatusText == "OK") { //Success && Response contains data
            $scope.userData = response.data.d;
        } else {            
            alert(response.data.StatusText); 
        }
    }, function (response) {    // Error or response contains NO data
        alert(response.statusText);
    });

    //Add New User....
    $scope.uploadPic = function (file) {
        if ($scope.User.FirstName != '' && $scope.User.LastName != '' && $scope.User.Age != '') {
            file.upload = Upload.upload({
                url: app_uri + '/Add',
                data: { user: $scope.User },
                file: file
            })
            .progress(function (evt) {
                //console.log('');
            }).success(function (data, status, headers, config) {
                if (data.d != null && data.StatusCode == 201 && data.StatusText == "Created") { //Success && Response contains data
                    $scope.userData = data.d;
                    $scope.clear();
                } else {
                    alert('Missing/incorrect User data!'); //Success && Response contains NO data
                }
            }).error(function (err) {
                alert('Error occured while adding new user');
            });
        }
        else {
            alert('Please Enter First Name, Last Name and Age !!');
        }
    }

    //Clear 'Add User' Textboxes....
    $scope.clear = function () {
        $scope.User.FirstName = '';
        $scope.User.LastName = '';
        $scope.User.Address = '';
        $scope.User.Age = '';
        $scope.User.Interests = '';
        $scope.picFile = '';
    };

    //Search Users
    $scope.searchedUserData = null;
    //event fires when click on textbox
    $scope.userSearch = function () {
        var inputMin = 1;
        if ($scope.searchVal.length >= inputMin) {
            var userSearchbox = angular.element(document.querySelector('#userAutoSearch'));
            userSearchbox.addClass('ui-autocomplete-loading');
            $http({
                method: 'GET',
                url: app_uri + '/Search?searchText=' + $scope.searchVal
                //data: $scope.User
            }).then(function successCallback(response) {
                // this callback will be called asynchronously
                // when the response is available
                if (response.data.d != null && response.data.StatusCode == 200 && response.data.StatusText == "OK") { //Success && Response contains data
                    $scope.searchedUserData = response.data.d;
                } else {
                    alert(response.data.StatusText); //Success && Response contains NO data
                }
                userSearchbox.removeClass('ui-autocomplete-loading');
            }, function errorCallback(response) {
                userSearchbox.removeClass('ui-autocomplete-loading');
                $scope.searchVal = '';
                // called asynchronously if an error occurs
                // or server returns response with an error status.
                alert(response.statusText);
            });
        }
        else {
            $scope.searchedUserData = null;
        }
    };

});
