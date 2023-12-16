Hashing.cs and Response.cs are in another library called SharedNamespace

Authenticator.cs:

	user inputs username

	manager gets username --> creates authenticationRequest with UserIdentity = username and Proof = null

	Authenticator.SendOTP_and_SaveToDB(AuthenticationRequest authRequest) is called with the request put in the parameter, this returns the username, otp (string), and result

	manager gets this back and send the otp to the user via email

	user inputs otp

	manager gets otp --> creates new authenticationRequest with UserIdentity = username and Proof = user inputted password

	Authenticator.Authenticate(AuthenticationRequest authRequest) is called

	if successful, this returns the principal (UserIdentity and Role) and result (hasError = false)



rn, its role based since i couldnt figure out ~109 where the values of result.ValuesRead is returned. since the value at ValuesRead[row][column] is a string, idrk what to do to make it a dictionary. im not completely sure on what the table for the user roles looks like rn, so idk how to implement it. lmk if u have any ideas for it!