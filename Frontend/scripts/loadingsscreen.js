let domonderwerpen,startGame,dataName,teamAName,teamBName,teamAAvatarId="",teamBAvatarId="",teamNameACard,teamNameBCard;
let quiz_ids = [];

const ontvangenOnderwerpen=(data)=>{
  //console.log(data[0].title)
  onderwerp=document.querySelector('.js-select');
  onderwerpHTML='';

  for (let i = 0; i < data.length; i++) {
    //console.log(data[i].title);
 /*    console.log(data[i].quiz_id); */
    let ids = {};
    const quiziddata = data[i];
    if(quiziddata.quiz_id){
      ids['quiz_id'] = quiziddata.quiz_id;
    }

    if (quiziddata.title){
      ids['title'] = quiziddata.title;
    }

    quiz_ids.push(ids);

    onderwerpHTML += `
    <select name="select" id="select" class=" u-grid-x-3 u-grid-y-1 js-select">
    <option>${data[i].title}</option>
    </select> 
   `;
    
  }
  onderwerp.innerHTML += onderwerpHTML;

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
//addEventListener voor button hierbij worsd er een Post naar de database gedaan
const buttonEvent=()=>{
  startGame=document.querySelector('.js-button_startGame');
  startGame.addEventListener('click', function() {
    console.log('in button event')
    //items opvragen uit local storage
    teamAName=localStorage.getItem('teamnaamA');
    teamBName=localStorage.getItem('teamnaamB');
    teamAAvatarId=localStorage.getItem('teamAvatarA');
    teamBAvatarId=localStorage.getItem('teamAvatarB');
    //console.log(teamAName,teamBName,teamAAvatarId,teamBAvatarId)
    //datastrucuur doorsturen
    dataName={teams:[{name:teamAName,avatar:{avatar_id:teamAAvatarId}},{name:teamBName,avatar:{avatar_id:teamBAvatarId}}]}
    //post naar database
    PostAPI(dataName);

    let e = document.getElementById("select");
    let aangeduidobject = e.options[e.selectedIndex].value;
  /*   console.log(aangeduidobject);
    console.log(quiz_ids); */
    for (var quiz_id in quiz_ids){
      if(!quiz_ids.hasOwnProperty(quiz_id)) continue;
      const everything = quiz_ids[quiz_id];
      const qid = everything.quiz_id;
     /*  console.log(qid); */
      const title = everything.title;
      if (title == aangeduidobject)
      {
        localStorage.setItem('quizid',qid);
      }
  
    }


    window.open("./frontend/vragen.html");
    
  });
}


//fetchPost
const fetchData2 = async function( dataName,method = "POST", body = null) {
    //console.log("in fetch")
	  return fetch(`https://mctproject2.azurewebsites.net/api/v1/game/BEF11CA2-3FB0-4BDF-90D2-2AD0BE4787E6`, {
	  method: method,
	  body: JSON.stringify(dataName),
	  headers: { "content-type": "application/json" }
	})
	  .then(r => r.json())
	  .then(data => data);
}; 
const PostAPI = async function(dataName, method = "POST", body = null) {
  try 
  {
    const dataURL = await fetchData2(dataName, method, body);
    console.log("gameid" + " " + dataURL.id);
    let gameid = dataURL.id 
    localStorage.SetItem('gameid', gameid);
    
    let id = localStorage.getItem('quizid');
    console.log("quizid" + " " + id);
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