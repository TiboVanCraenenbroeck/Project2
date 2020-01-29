let domonderwerpen,
  domteamRaketA,
  domteamRaketB,
  domStartGame,
  domOptions,
  domOnderwerp,
  domTeamNameACard,
  domTeamNameBCard,
  startGame,
  dataName,
  teamAName,
  teamBName,
  teamAAvatarId = "",
  teamBAvatarId = "",
  teamNameACard,
  teamNameBCard,
  onderwerpId,
  teamAAvatarName,
  gameIdData,
  teamBAvatarName;
const url = "https://mctproject2.azurewebsites.net/api/v1/subjects";

const ontvangenOnderwerpen = data => {
  onderwerpHTML = "";

  for (let i = 0; i < data.length; i++) {
    onderwerpHTML += `<option value="${data[i].quiz_id}">${data[i].title}</option>`;
  }

  domOnderwerp.innerHTML += onderwerpHTML;
  //changeOptionVal();
};

//veranderen van de namen in de cards
const changeName = () => {
  domTeamNameACard.innerHTML = teamAName;
  domTeamNameBCard.innerHTML = teamBName;
};

//addEventListener voor button hierbij worsd er een Post naar de database gedaan
const buttonEvent = () => {
  domStartGame.addEventListener("click", async function() {
    console.log(domOptions.value);
    console.log(teamAAvatarId, teamBAvatarId);
    //datastrucuur doorsturen
    dataName = {
      teams: [
        {
          name: teamAName,
          avatar: { avatar_id: `${teamAAvatarId}` }
        },
        {
          name: teamBName,
          avatar: { avatar_id: `${teamBAvatarId}` }
        }
      ]
    };
    onderwerpId = domOptions.value;
    // Controleer of de gebruiker een onderwerp geselecteerd heeft
    if (onderwerpId != "kies een onderwerp") {
      //post naar database
      localStorage.setItem("quizid", onderwerpId);
      const gameIdData = await PostAPI(onderwerpId, dataName);
      if (gameIdData) {
        window.location.href = "vragen.html";
      }
    } else {
      alert("Gelieve een onderwerp te selecteren");
    }
  });
};
const laadGekozenAvatars = () => {
  // raket vervangen
  let raketAHTML = "";
  let raketBHTML = "";
  raketAHTML = `      
      <div class="c-olievlek c-img__center js-raketA">
                    <img class="c-img-scale " data-avatarId="${teamAAvatarId}" src="https://aikovanryssel.github.io/project2IMG/img/raket/svg/${teamAAvatarName}.svg"  alt="${teamAAvatarName}">
               </div>`;
  raketBHTML = `<div class="c-olievlek c-img__center js-raketB">
  <img class="c-img-scale "data-avatarId="${teamBAvatarId}" src="https://aikovanryssel.github.io/project2IMG/img/raket/svg/${teamBAvatarName}.svg" alt="${teamBAvatarName}">
</div>`;
  domteamRaketA.innerHTML = raketAHTML;
  domteamRaketB.innerHTML = raketBHTML;
};

//fetchPost
const fetchData2 = async function(
  onderwerpId,
  dataName,
  method = "POST",
  body = null
) {
  return fetch(
    `https://mctproject2.azurewebsites.net/api/v1/game/${onderwerpId}`,
    {
      method: method,
      body: JSON.stringify(dataName),
      headers: { "content-type": "application/json" }
    }
  )
    .then(r => r.json())
    .then(data => data);
};
const PostAPI = async function(
  onderwerpId,
  dataName,
  method = "POST",
  body = null
) {
  try {
    const dataURL = await fetchData2(onderwerpId, dataName, method, body);
    //console.log(dataURL);
    gameIdData = dataURL.id;
    await gameIdData;
    localStorage.setItem("gameid", gameIdData);
    console.log(gameIdData);
    return gameIdData;
  } catch (error) {
    console.log(error);
  }
};
//fetchonderwerpen
const fetchData = url => {
  // Add a few headers - UITZONDERING
  let customHeaders = new Headers();
  customHeaders.append("Accept", "application/json");

  return fetch(url, { headers: customHeaders })
    .then(r => r.json())
    .then(data => data);
};
//async function
const getApi = async function(SERVER_ENDPOINT) {
  const data = await fetchData(SERVER_ENDPOINT);
  //console.log(data);
  ontvangenOnderwerpen(data);
};

const init = () => {
  //items opvragen uit local storage
  teamAName = localStorage.getItem("teamnaamA");
  teamBName = localStorage.getItem("teamnaamB");

  teamAAvatarId = localStorage.getItem("teamAvatarA");
  teamBAvatarId = localStorage.getItem("teamAvatarB");

  teamAAvatarName = localStorage.getItem("teamAvatarNameA");
  teamBAvatarName = localStorage.getItem("teamAvatarNameB");
  //dom items
  domonderwerpen = document.querySelector(".js-select");
  domteamRaketA = document.querySelector(".js-raketA");
  domteamRaketB = document.querySelector(".js-raketB");
  domStartGame = document.querySelector(".js-button_startGame");
  domOptions = document.querySelector("#select");
  domOnderwerp = document.querySelector(".js-select");
  domTeamNameACard = document.querySelector(".js-teamA");
  domTeamNameBCard = document.querySelector(".js-teamB");
};
document.addEventListener("DOMContentLoaded", function() {
  init();
  changeName();
  buttonEvent();
  laadGekozenAvatars();
  getApi(url);
});
