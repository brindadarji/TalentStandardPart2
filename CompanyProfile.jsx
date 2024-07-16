import React from 'react';
import { Loader, Icon } from 'semantic-ui-react';
import Cookies from 'js-cookie';
import { BodyWrapper, loaderData } from '../Layout/BodyWrapper.jsx';

export default class CompanyProfile extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            employerName: '',
            Address: '',
            Phone: '',
            Email: '',
            ProfilePhoto:'',

        };
    }

    componentDidMount() {
        this.getCompanyProfile()
    }

    getCompanyProfile() {
        const cookies = Cookies.get('talentAuthToken');
        try {
            $.ajax({
                url: 'https://talentservicesprofile20240417233610.azurewebsites.net/profile/profile/getEmployerProfile',
                headers: {
                    'Authorization': 'Bearer ' + cookies,
                    'Content-Type': 'application/json'
                },
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                success: function (res) {
                    let employerData = null;
                    if (res.employer) {
                        employerData = res.employer
                        console.log("employerData", employerData)
                        const name = employerData.companyContact.name;
                        const address = employerData.companyContact.location.country + ", " + employerData.companyContact.location.city;
                        const phone = employerData.companyContact.phone;
                        const email = employerData.companyContact.email;
                        const profilephoto = employerData.profilePhotoUrl;
                        this.setState({
                            employerName: name,
                            Address: address,
                            Phone: phone,
                            Email: email,
                            ProfilePhoto: profilephoto
                        });
                    }
                }.bind(this),
                error: function (res) {
                    console.log(res.status)
                }
            })
        }
        catch(error) {
            console.log("Error fetching employer Data")
        }
    }


    render() {
        const { employerName, Address, Phone, Email, ProfilePhoto } = this.state;
        return(
        <div className="ui card">
            <div className="content">
                <div className="center aligned author">
                        <Icon name='file image outline' className="circular icon company-photo"></Icon>
                </div>
                <div className="center aligned header">{employerName}</div>
                <div className="center aligned meta">{Address}</div>
                <div className="center aligned description">
                    We currently do not have specific skills that we desire.
                </div>
            </div>
            <div className="extra content">
                    <div>
                        <Icon name='phone'></Icon>:{ Phone }<br></br>
                        <Icon name='mail'></Icon>: { Email }
                    </div>
              
            </div>
        </div>
        )
    }
}