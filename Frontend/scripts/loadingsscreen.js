let domonderwerpen,startGame,dataName,teamAName,teamBName,teamAAvatarId="31fdb58c-69e2-4abe-b011-f612928cec9f",teamBAvatarId="40bdd510-398e-4b79-97e1-d4a0be1fd1a4",teamNameACard,teamNameBCard,onderwerpId;

const ontvangenOnderwerpen=(data)=>{
  //console.log(data[0].title)
  onderwerp=document.querySelector('.js-select');
  onderwerpHTML='';

  for (let i = 0; i < data.length; i++) {
    onderwerpHTML += `<option value="${data[i].quiz_id}">${data[i].title}</option>`;  
  }

  onderwerp.innerHTML += onderwerpHTML;
  //changeOptionVal();
}

//veranderen van de namen in de cards
const changeName=(teamA, teamB)=>{
  teamNameACard=document.querySelector('.js-teamA');
  teamNameBCard=document.querySelector('.js-teamB');

  let teamNameAHTML = '';
  let teamNameBHTML = '';
  teamNameAHTML = `<h1 class="c-teamname js-teamA">${teamA}</h1>`
  teamNameBHTML = `<h1 class="c-teamname js-teamB">${teamB}</h1>`


  teamNameACard.innerHTML = teamNameAHTML;
  teamNameBCard.innerHTML = teamNameBHTML;

}
/* const changeOptionVal=()=>{
  options=document.querySelector('#select');
  options.addEventListener('change', function() {
    console.log(options.value);
  })
} */
//addEventListener voor button hierbij worsd er een Post naar de database gedaan
const buttonEvent=()=>{
  startGame=document.querySelector('.js-button_startGame');
  options=document.querySelector('#select');

  startGame.addEventListener('click', function() {
    console.log('in button event')
    console.log(options.value);
    //items opvragen uit local storage
    teamAName=localStorage.getItem('teamnaamA');
    teamBName=localStorage.getItem('teamnaamB');
    teamAAvatarId=localStorage.getItem('teamAvatarA');
    teamBAvatarId=localStorage.getItem('teamAvatarB');
    //console.log(teamAName,teamBName,teamAAvatarId,teamBAvatarId)
    //datastrucuur doorsturen
    dataName={"teams":[{"name":`${teamAName}`,"avatar":{"avatar_id":`${teamAAvatarId}`}},{"name":`${teamBName}`,"avatar":{"avatar_id":`${teamBAvatarId}`}}]}
    onderwerpId=options.value;
    //post naar database
    PostAPI(onderwerpId,dataName);
  });
}


//fetchPost
const fetchData2 = async function( onderwerpId,dataName,method = "POST", body = null) {
    //console.log("in fetch")
/*     console.log(onderwerpId)*/	  
    return fetch(`https://mctproject2.azurewebsites.net/api/v1/game/${onderwerpId}`, {
	  method: method,
	  body: JSON.stringify(dataName),
	  headers: { "content-type": "application/json" }
	})
	  .then(r => r.json())
	  .then(data => data);
}; 
const PostAPI = async function(onderwerpId,dataName, method = "POST", body = null) {
  try 
  {
    const dataURL = await fetchData2(onderwerpId,dataName, method, body);
    console.log(dataURL)
    /* console.log("gameid" + " " + dataURL.id);
    let gameid = dataURL.id 
    localStorage.setItem('gameid', gameid);
    
    let id = localStorage.getItem('quizid');
    console.log("quizid" + " " + id); */
  } 
  catch (error) 
  {
	  console.log(error);
	}
};
//fetchonderwerpen
const fetchData = (url) => {
  // Add a few headers - UITZONDERING
  let customHeaders = new Headers();
  customHeaders.append('Accept', 'application/json');

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
document.addEventListener('DOMContentLoaded', function()
{
  changeName(localStorage.getItem('teamnaamA'),localStorage.getItem('teamnaamB'));
  buttonEvent();
  let test="https://mctproject2.azurewebsites.net/api/v1/subjects"
  getApi(test);

  domonderwerpen = document.querySelector('.js-select')
});