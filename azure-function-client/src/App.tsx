import React, { useCallback, useEffect, useRef, useState } from 'react';
import axios from 'axios';
import { useIsAuthenticated, useMsal } from "@azure/msal-react";
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Grid from '@mui/material/Grid';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import { loginRequest } from "./msalConfig";
import './App.css';

function App() {
  const { instance, accounts } = useMsal();
  const isAuthenticated = useIsAuthenticated();
  const [isHaveActiveAccount, setActiveAccount] = useState(false);
  const [noteData, setNoteData] = useState<Array<any>>([]);
  const [open, setOpen] = useState(false);
  const [message, setMessage] = useState('');
  const [title, setTitle] = useState('');

  const handleClickOpen = () => {
    setOpen(true);
  };

  const handleClose = () => {
    setOpen(false);
  };

  const onMessageChange = (e: any) => setMessage(e.target.value);
  const onTitleChange = (e: any) => setTitle(e.target.value);

  const login = useCallback(async () => {
    try {
      const loginResult = await instance.loginPopup(loginRequest);
      instance.setActiveAccount(loginResult.account);
      setActiveAccount(!!loginResult.account);
    } catch (err) {
      console.error(err);
    }
  }, [instance]);

  const logout = useCallback(async () => {
    const activeAccount = instance.getActiveAccount();
    await instance.logoutRedirect({
      account: activeAccount
    });
  }, [instance]);

  const addNote = () => {
    if (isHaveActiveAccount && message && title) {
      instance.acquireTokenSilent(loginRequest).then(result => {
        const accessToken = result.accessToken;
        console.log(accessToken);
        axios.post("https://af-demo-berv-1.azurewebsites.net/api/CreateToDoTrigger", {
          message,
          title
        }, {
          headers: {
            'Authorization': `Bearer ${accessToken}`
          }
        }).then(response => {
          console.log(response);
          setOpen(false);
          getNote();
          Promise.resolve();
        }).catch(error => {
          console.error(error);
          Promise.reject(error);
        })
      }).catch(err => {
        console.error(err);
        Promise.reject(err);
      });
    } else {
      Promise.reject("Un-authorized or not fill data");
    }
  };

  const renderLogin = useCallback(() => {
    const activeAccount = instance.getActiveAccount();
    if (!isAuthenticated) {
      return (<Button variant="contained" onClick={login}>Login</Button>);
    } else {
      return (
        <>
          <Typography variant="h4">Account List</Typography>
          <Grid container spacing={2}>
            {accounts.map((account, index) => (
              <Grid item xs={4}>
                <Card key={index}>
                  <CardContent>
                    <Typography variant="body2">{account.username}</Typography>
                  </CardContent>
                  <CardActions>
                    <Button size="small" disabled={activeAccount?.username === account.username} onClick={() => {
                      instance.setActiveAccount(account);
                      setActiveAccount(true);
                    }}>Set as active</Button>
                  </CardActions>
                </Card>
              </Grid>))
            }
            <Grid item xs={12}>
              <Button variant="contained" color="error" onClick={logout}>Logout</Button>
            </Grid>
          </Grid>
        </>
      );
    }
  }, [accounts, instance, isAuthenticated]);

  const renderNote = useCallback(() => {
    if (isHaveActiveAccount) {
      return (<>
        <Button onClick={handleClickOpen} variant="contained">Add Note</Button>
        {noteData.length > 0 ? (noteData.map((data, index) => (
            <Card key={index}>
              <CardContent>
                <Typography variant="h5">{data.title}</Typography>
                <Typography variant="h6">{data.message}</Typography>
              </CardContent>
            </Card>
        ))) : <Typography variant="h5">No Data</Typography>}
        <Dialog open={open} onClose={handleClose}>
          <DialogTitle>Add Note</DialogTitle>
          <DialogContent>
            <DialogContentText>
              Please fill this fields.
            </DialogContentText>
            <TextField
              autoFocus
              margin="dense"
              required
              id="title"
              label="Title"
              type="text"
              fullWidth
              onChange={onTitleChange}
              value={title}
              variant="standard"
            />
            <TextField
              autoFocus
              margin="dense"
              id="message"
              label="Message"
              type="text"
              fullWidth
              required
              onChange={onMessageChange}
              value={message}
              variant="standard"
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={handleClose}>Cancel</Button>
            <Button onClick={addNote}>Add Note</Button>
          </DialogActions>
        </Dialog>
      </>);
    } else {
      return (<div>Please Login First</div>);
    }
  }, [isHaveActiveAccount, noteData, open, title, message]);

  const getNote = () => {
    instance.acquireTokenSilent(loginRequest).then(result => {
      const accessToken = result.accessToken;
      console.log(accessToken);
      axios.get("https://af-demo-berv-1.azurewebsites.net/api/GetToDoTrigger", {
        headers: {
          'Authorization': `Bearer ${accessToken}`
        }
      }).then(response => {
        console.log(response.data);
        setNoteData(response.data);
      }).catch(error => {
        console.error(error);
      })
    }).catch(err => {
      console.error(err);
    });
  }

  useEffect(() => {
    const account = instance.getActiveAccount();
    setActiveAccount(!!account);
  }, []);

  useEffect(() => {
    if (isHaveActiveAccount) {
      getNote();
    }
  }, [isHaveActiveAccount]);

  return (
    <div className="App">
      <header className="App-header">
        <Typography variant="h2">React Notes Azure Functions</Typography>
        <Grid container spacing={2}>
          <Grid item xs={6}>
            {renderLogin()}
          </Grid>
          <Grid item xs={6}>
            {renderNote()}
          </Grid>
        </Grid>
      </header>
    </div>
  );
}

export default App;
