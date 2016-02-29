var SearchPanel = React.createClass({
    render: function() {
	    return (
            <form className="form-inline">
				<div className="form-group">
					<label className="sr-only" htmlFor="filter">Filter</label>
					<div className="input-group">
						<div className="input-group-addon"><span className="glyphicon glyphicon-filter" aria-hidden="true"></span></div>
						<input ref="search" name="filter" type="text" value={this.props.search} onChange={this.onSearchChanged} className="form-control" />
						{this.props.search ? <div className="input-group-addon" onClick={this.props.onClearSearch }><span className="glyphicon glyphicon-remove" aria-hidden="true"></span></div> : null}
					</div>
				</div>
            </form>
	    );
    },
    onSearchChanged: function() {
        var query = React.findDOMNode(this.refs.search).value;
        this.props.onSearchChanged(query);
    }
});

var ContactTableRow = React.createClass({
    render: function() {
        return (
            <tr>
                <td>{this.props.contact.lastName}, {this.props.contact.firstName}</td>
                <td>{this.props.contact.email}</td>
                <td>{this.props.contact.phone}</td>
                <td>{this.props.contact.address}</td>
                <td><a href="#" onClick={this.onClick}>Edit</a></td>
            </tr>
        );
    },
    onClick: function() {
        this.props.handleEditClickPanel(this.props.contact.id);
    }
});

var ContactTable = React.createClass({
    render: function() {
        var rows = [];
        this.props.contacts.forEach(function(contact) {
            rows.push(<ContactTableRow key={contact.id} contact={contact} handleEditClickPanel={this.props.handleEditClickPanel} />);
        }.bind(this));
    	return (
			<div className="table-responsive">
				<table className="table table-hover">
					<thead>
						<tr>
							<th>Name</th>
							<th>Email</th>
							<th>Phone</th>
							<th>Address</th>
							<th></th>
						</tr>
					</thead>
					<tbody>{rows}</tbody>
				</table>
			</div>
        );
    }
});

var ContactForm = React.createClass({
    render: function() {
        return(
            <form onSubmit={this.props.handleSubmitClick}>
				<div className="form-group">
					<label forHtml="firstName">First Name</label>
					<input ref="firstName" name="firstName" type="text" value={this.props.contact.firstName} maxLength="15" required onChange={this.onChange} className="form-control" />
				</div>
				<div className="form-group">
	               <label forHtml="lastName">Last Name</label>
					<input ref="lastName" name="lastName" type="text" value={this.props.contact.lastName} maxLength="15" required onChange={this.onChange} className="form-control" />
				</div>
				<div className="form-group">
					<label forHtml="email">Email</label>
					<input ref="email" name="email" type="email" value={this.props.contact.email} maxLength="30" required onChange={this.onChange} className="form-control" />
				</div>
				<div className="form-group">
					<label forHtml="phone">Phone</label>
					<input ref="phone" name="phone" type="tel" value={this.props.contact.phone} maxLength="15" onChange={this.onChange} className="form-control" />
				</div>
				<div className="form-group">
					<label forHtml="address">Address</label>
					<input ref="address" name="address" type="text" value={this.props.contact.address} maxLength="50" onChange={this.onChange} className="form-control" />
				</div>
                <button type="submit" className="btn btn-primary">{this.props.contact.id ? "Save" : "Add"}</button>&nbsp;
					{this.props.contact.id ? <button onClick={this.props.handleDeleteClick } className="btn btn-danger">Delete</button> : null}&nbsp;
					{this.props.contact.id ? <button onClick={this.props.handleCancelClick } className="btn btn-default">Cancel</button> : null}
					{this.props.message ? <div>{this.props.message}</div> : null}
			</form>
        );
    },
    onChange: function() {
        var firstName = React.findDOMNode(this.refs.firstName).value;
        var lastName = React.findDOMNode(this.refs.lastName).value;
        var email = React.findDOMNode(this.refs.email).value;
        var phone = React.findDOMNode(this.refs.phone).value;
        var address = React.findDOMNode(this.refs.address).value;
        this.props.handleChange(firstName, lastName, email, phone, address);
    }
});

var ContactPanel = React.createClass({
    getInitialState: function() {
        return {
            contacts: [],
            editingContact: {
                firstName:"",
				lastName:"",
				email:"",
				phone:"",
                address:""
            },
            search:"",
            message:""
        };
    },
    render: function() {
    	return (
			<div className="container-fluid">
				<h3 className="text-primary text-center">React Address Book</h3>
				<div className="row">
					<div className="col-md-8">
						<div className="panel panel-info">
							<div className="panel-heading"><span className="glyphicon glyphicon-th-list" aria-hidden="true"></span></div>
							<div className="panel-body">
								<SearchPanel search={this.state.search}
											 onSearchChanged={this.onSearchChanged}
											 onClearSearch={this.onClearSearch} />
								<ContactTable contacts={this.state.contacts} handleEditClickPanel={this.handleEditClickPanel} />
							</div>
						</div>
					</div>
					<div className="col-md-4">
						<div className="panel panel-success">
							<div className="panel-heading"><span className="glyphicon glyphicon-user" aria-hidden="true"></span></div>
							<div className="panel-body">
								<ContactForm contact={this.state.editingContact}
										 message={this.state.message}
										 handleChange={this.handleChange}
										 handleSubmitClick={this.handleSubmitClick}
										 handleCancelClick={this.handleCancelClick}
										 handleDeleteClick={this.handleDeleteClick} />
							</div>
						</div>
					</div>
				</div>
			</div>
        );
    },
    componentDidMount: function() {
        this.reloadContacts("");
    },
    onSearchChanged: function(query) {
        if (this.promise) {
	        clearInterval(this.promise);
        }
        this.setState({
            search: query
        });
        this.promise = setTimeout(function () {
            this.reloadContacts(query);
        }.bind(this), 200);
    },
    onClearSearch: function() {
        this.setState({
            search: ""
        });
        this.reloadContacts("");
    },
    handleEditClickPanel: function(id) {
        var contact = $.extend({}, this.state.contacts.filter(function(x) {
            return x.id === id;
        })[0] );
        
        this.setState({
            editingContact: contact,
            message: ""
        });
    },
    handleChange: function (firstName, lastName, email, phone, address) {
    	var validationError = undefined;
		if (firstName.length < 1 || lastName.length < 1 || email.length < 1) {
			validationError = "Missing required fields";
		}
        this.setState({
            editingContact: {
                firstName: firstName,
                lastName: lastName,
                email: email,
                phone: phone,
				address: address,
				id: this.state.editingContact.id,
				validationError: validationError
            }
        });
    },
    handleCancelClick: function() {
        this.setState({
            editingContact: {}
        });
    },    
    reloadContacts: function(query) {
        $.ajax({
            url: this.props.url+"Search?query="+query,
            dataType: "json",
            cache: false,
            success: function(data) {
                this.setState({
                    contacts: data,
                    search: query
                });
            }.bind(this),
            error: function(xhr, status, err) {
                console.error(this.props.url, status, err.toString());
                this.setState({
                    message: err.toString()
                });
            }.bind(this)
        });
    },
    handleSubmitClick: function(e) {
    	e.preventDefault();
		if (this.state.editingContact.validationError) {
			this.setState({
				message: this.state.editingContact.validationError
			});
			return;
		}
        if(this.state.editingContact.id) {
            $.ajax({
                url: this.props.url+this.state.editingContact.id,
                dataType: "json",
                method: "PUT",
                data:this.state.editingContact,
                cache: false,
                success: function() {
                    this.setState({
                        message: "Successfully updated contact"
                    });
                    this.reloadContacts("");
                }.bind(this),
                error: function(xhr, status, err) {
                    console.error(this.props.url, status, err.toString());
                    this.setState({
                        message: err.toString()
                    });
                }.bind(this)
            });
        } else {
            $.ajax({
                url: this.props.url,
                dataType: "json",
                method: "POST",
                data:this.state.editingContact,
                cache: false,
                success: function() {
                    this.setState({
                        message: "Successfully added contact"
                    });
                    this.reloadContacts("");
                }.bind(this),
                error: function(xhr, status, err) {
                    console.error(this.props.url, status, err.toString());
                    this.setState({
                        message: err.toString()
                    });
                }.bind(this)
            });
        }
        this.setState({
            editingContact: {}
        });
    },
    handleDeleteClick: function(e) {
        e.preventDefault();
        $.ajax({
            url: this.props.url+this.state.editingContact.id,
            method: "DELETE",
            cache: false,
            success: function() {
                this.setState({
                    message: "Successfully deleted contact",
                    editingContact: {}
                });
                this.reloadContacts("");
            }.bind(this),
            error: function(xhr, status, err) {
                console.error(this.props.url, status, err.toString());
                this.setState({
                    message: err.toString()
                });
            }.bind(this)
        });
    }
});

React.render(<ContactPanel url="/api/contacts/" />, document.getElementById("content"));
