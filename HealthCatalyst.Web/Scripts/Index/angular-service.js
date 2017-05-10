
app.factory('UserService', function ($http) {
    var fac = {};
    fac.GetAllUsers = function () {
        return $http.get(app_uri + '/All');
    }
    return fac;
});
