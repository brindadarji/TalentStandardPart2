import React from 'react';
import ReactDOM from 'react-dom';
import Cookies from 'js-cookie';
import ReactPlayer from 'react-player';
import { Icon, Image } from 'semantic-ui-react'

export default class TalentCardDetail extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            TalentDetails: null,
        };
    }

    componentDidMount() {
        this.getTalentDetails();
    }

    componentDidUpdate(prevProps) {
        if (this.props.selectedTalentId !== prevProps.selectedTalentId) {
            this.getTalentDetails();
        }
    }

    getTalentDetails() {
        const { selectedTalentId } = this.props;
        const cookies = Cookies.get('talentAuthToken');
        try {
            $.ajax({
                url: 'https://talentservicesprofile20240417233610.azurewebsites.net/profile/profile/getTalentProfile/?id=' + selectedTalentId,
                headers: {
                    'Authorization': 'Bearer ' + cookies,
                    'Content-Type': 'application/json'
                },
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                success: function (res) {
                    if (res.success) {
                        this.setState({ TalentDetails: res.data });
                    }
                }.bind(this),
                error: function (res) {
                    console.log(res.status);
                }
            })
        }
        catch (error) {
            console.log("Error fetching Talent Data");
        }
    }

    renderSkills(skills) {
        return skills.map((skill) => (
            <div key={skill.id} className="skill-item skills-div">
                <a className="ui blue basic label">{skill.name}</a>
            </div>
        ));
    }

    getCurrentEmployerAndPosition(experience) {
        if (!experience || experience.length === 0) {
            return { company: 'N/A', position: 'N/A' };
        }
        const sortedExperience = experience.sort((a, b) => new Date(b.start) - new Date(a.start));
        return {
            company: sortedExperience[0].company,
            position: sortedExperience[0].position
        };
    }


    render() {
        const { selectedTalentId, handleBackToCards } = this.props;
        const { TalentDetails } = this.state;
        if (!TalentDetails) {
            return <div>Loading...</div>;
        }
        const { company, position } = this.getCurrentEmployerAndPosition(TalentDetails.experience);
        const defaultAvatar = "https://react.semantic-ui.com/images/avatar/large/matthew.png";

        return (
            <div className="ui raised link job card" key={selectedTalentId}>
                <div className="content">
                    <div className="header">
                        {TalentDetails.firstName} {TalentDetails.lastName}
                        <span> <Icon name='star' className='talentfeed_star' size='large'></Icon></span>
                    </div>
                </div>
                <div className="ui sixteen wide column">
                    <table className="center aligned ui single line table">
                        <tbody>
                        <tr>
                            <td className="photo-td">
                                <div className="image-container">
                                    <img className='ui image profile-photo' src={TalentDetails.profilePhotoUrl || defaultAvatar}></img>
                                </div>
                            </td>
                            <td className='talentdetails-td'>
                                <div className='talentdetails-div'>
                                    <p><b>Talent snapshot</b></p>
                                    <p><b>CURRENT EMPLOYER</b><span className='details-span'>{company}</span></p>
                                    <p><b>VISA STATUS</b>
                                        <span className='details-span'>{TalentDetails.visaStatus}</span></p>
                                    <p><b>POSITION</b><span className='details-span'>{position}</span></p>
                                </div>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </div>
                <div className="ui sixteen wide column">
                    <table className="center aligned ui single line table">
                        <tbody>
                        <tr>
                            <td><Icon className='video large icon' onClick={handleBackToCards}></Icon></td>
                            <td><Icon className='file pdf outline large icon'></Icon></td>
                            <td><a href={TalentDetails.linkedAccounts.linkedIn} target="_blank" rel="noopener noreferrer"><Icon className='linkedin large icon'></Icon></a></td>
                            <td><a href={TalentDetails.linkedAccounts.github} target="_blank" rel="noopener noreferrer"><Icon className='github large icon'></Icon></a></td>
                        </tr>
                        </tbody>
                    </table>
                </div>
                <div className="extra content">
                    <div className="left floated skils-block">
                        {this.renderSkills(TalentDetails.skills)}
                    </div>
                </div>
            </div>
        )
    }
}