// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Generate a random salary between 18000 and 32000, including both 18000 and 32000
function generateRandomNumber(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

////Method to generate a random date
function randomDate(start, end) {
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
}

document.querySelector('#randomGeneratorInput').addEventListener("click", function () {

    document.querySelector("#createEmployeeManually").style.display = "none";
    document.querySelector("#successfullyCreatedEmployee").innerHTML = "Employee created succesfully";

    axios.get(`https://randomuser.me/api/`)
        .then(function (response){
            let data = response.data.results[0]
            console.log(data)
            //To enter the user image randomly, is something complex, we have to generate a hidden input in the front end with a boolean condition, to differentiate whether the image enters manually, or through the Fill Random user button via js.
            document.querySelector("#boolPicture").value = "True";
            //If we use the Fill Random User button, the variable changes to true and the UploadProfilePic method is executed differently.
            document.querySelector("#randomPicture").value = `${data.picture.medium}`;
            document.querySelector("#randomName").value = `${data.name.first}`;
            document.querySelector("#randomSurname").value = `${data.name.last}`;

            let randomDateTime = randomDate(new Date(2012, 0, 1), new Date());
            randomDateTime = randomDateTime.toLocaleDateString().split("/");
            for (let i = 0; i < randomDateTime.length; i++) {
                if (randomDateTime[i].length == 1) {
                    randomDateTime[i] = "0" + randomDateTime[i]
                }
            }
            let fuckingFinalDate = randomDateTime[2] + '-' + randomDateTime[1] + '-' + randomDateTime[0]
            document.querySelector("#randomDate").value = `${fuckingFinalDate}`;

            document.querySelector("#randomGrossSalary").value = `${generateRandomNumber(20000, 34000)}`;
            document.querySelector("#randomNetSalary").value = `${generateRandomNumber(18000, 32000)}`;
            let randomDepartmentId = Math.floor(Math.random() * document.querySelectorAll("#randomDepartmentId option").length);
            console.log(randomDepartmentId)
            document.querySelector("#randomDepartmentId").selectedIndex = randomDepartmentId;

            


        })
        .catch(function (error) {
            console.log(error);
        });
})