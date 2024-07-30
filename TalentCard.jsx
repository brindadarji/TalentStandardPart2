import React from 'react';
import ReactPlayer from 'react-player';
import PropTypes from 'prop-types'
import { Popup, Icon } from 'semantic-ui-react'
import Cookies from 'js-cookie';
import { BodyWrapper, loaderData } from '../Layout/BodyWrapper.jsx';
import TalentCardDetail from './TalentCardDetail.jsx';

export default class TalentCard extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            TalentData: [],
            selectedTalentId: null,
            prevId: null,
            backToCards: false,
        };

        this.handleBackToCards = this.handleBackToCards.bind(this);
    };

  
    componentDidMount() {
        this.getTalentData();
    }

    getTalentData() {
        const cookies = Cookies.get('talentAuthToken');
        try {
            $.ajax({
                url: 'https://talentservicesprofile20240417233610.azurewebsites.net/profile/profile/getTalent',
                headers: {
                    'Authorization': 'Bearer ' + cookies,
                    'Content-Type': 'application/json'
                },
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                success: function (res) {
                    let TalentData = null;
                    if (res.success) {
                        TalentData = res.data;
                        this.setState({ TalentData : TalentData });
                    }
                }.bind(this),
                error: function (res) {
                    console.log(res.status);
                }
            })
          
        }
        catch (error) {
            console.log("Error fetching Talent Data")
        }
    }

    renderNoTalentCard() {
        const { TalentData } = this.state;
        if (TalentData === null) {
            return (
                <div><p>There are no talents found for your recruitment company.</p></div>
            )
        }
    }

    handleIconClick(selectedTalentId) {
        this.setState({ selectedTalentId });
    }

    handleBackToCards() {
        this.setState({ selectedTalentId: null });
    }

    renderTalentDetail() {
        const { selectedTalentId } = this.state;
        return (
            <TalentCardDetail selectedTalentId={selectedTalentId} handleBackToCards={this.handleBackToCards}></TalentCardDetail>
        )
    }

    renderSkills(skills) {
        return skills.map((skill, index) => (
            <div key={index} className="skill-item skills-div">
                <a className="ui blue basic label">{skill}</a>
            </div>
        ));
    }

    renderTalentcard() {
        const { TalentData, selectedTalentId } = this.state;
            return (
                TalentData.map((talent) => (
                    <div className="ui raised link job card" key={talent.id}>
                        <div className="content">
                            <div className="header">
                                {talent.name}
                                <span> <Icon name='star' className='talentfeed_star' size='large'></Icon></span>
                            </div>
                        </div>
                        <div className="video-container">
                            <video controls className="video-player">
                                <source src={talent.videoUrl} type="video/mp4" />
                                Your browser does not support the video tag.
                            </video>
                        </div>
                        <div className="ui sixteen wide column">
                            <table className="center aligned ui single line table">
                                <tbody>
                                <tr>
                                    <td><Icon className='user large icon' onClick={() => this.handleIconClick(talent.id)}></Icon></td>
                                    <td><Icon className='file pdf outline large icon'></Icon></td>
                                    <td><Icon className='linkedin large icon'></Icon></td>
                                    <td><Icon className='github large icon'></Icon></td>
                                </tr>
                                </tbody>
                            </table>
                        </div>
                        <div className="extra content">
                            <div className="left floated skils-block">
                                {this.renderSkills(talent.skills)}
                            </div>
                        </div>
                    </div>
                ))
            )
    }

    render() {
        const { selectedTalentId, backToCards } = this.state;
        return (
            <div className="scrollable-talent-cards">
                {selectedTalentId ? this.renderTalentDetail() : this.renderTalentcard()}
            </div>
        )  
    }
}

