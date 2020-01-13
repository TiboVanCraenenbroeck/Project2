

const highscoreFilter=(data)=>{
    let highScoreHTML='',teamHTML='';
    let scores=document.querySelector('.js-punten');
    let teams=document.querySelector('.js-team')

    for (let i = 0; i < data.length; i++) {
        let naam = data[i].name;
        console.log(naam)
        let score=data[i].score
        console.log(score);
        teamHTML += `
        <div class="u-grid-x-1 u-grid-y-${i} js-team">
                <div class="c-img__start">
                  <p class="c-zero_margin ">${naam}</p>
                </div>
              </div>`;
        highScoreHTML += `
        <div class="u-grid-x-1 u-grid-y-${i} js-punten">
                <p class="c-text__end c-zero_margin">${score}</p>
              </div>`;
    }
    scores.innerHTML = highScoreHTML;
    teams.innerHTML = teamHTML;
};

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
    console.log(data);
    highscoreFilter(data);
}; 

document.addEventListener('DOMContentLoaded', function()
{
  let test="https://mctproject2.azurewebsites.net/api/v1/Highscores"
  getApi(test);
  domonderwerpen = document.querySelector('.js-select')
});