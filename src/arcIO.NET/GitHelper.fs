namespace arcIO.NET

open System.Diagnostics

module GitHelper =

    /// Executes Git command and returns git output.
    let executeGitCommandWithResponse (repoDir : string) (command : string) =

        let log = Logging.createLogger "ExecuteGitCommandLog"

        let procStartInfo = 
            ProcessStartInfo(
                WorkingDirectory = repoDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = "git",
                Arguments = command
            )
        
        let outputs = System.Collections.Generic.List<string>()
        let outputHandler (_sender:obj) (args:DataReceivedEventArgs) = 
            if (args.Data = null |> not) then
                if args.Data.ToLower().Contains ("error") then
                    log.Error($"GIT: {args.Data}")    
                elif args.Data.ToLower().Contains ("trace") then
                    log.Trace($"GIT: {args.Data}")   
                else
                    outputs.Add(args.Data)
                    log.Info($"GIT: {args.Data}")
        
        let errorHandler (_sender:obj) (args:DataReceivedEventArgs) =  
            if (args.Data = null |> not) then
                let msg = args.Data.ToLower()
                if msg.Contains ("error") || msg.Contains ("fatal") then
                    log.Error($"GIT: {args.Data}")    
                elif msg.Contains ("trace") then
                    log.Trace($"GIT: {args.Data}")   
                else
                    outputs.Add(args.Data)
                    log.Info($"GIT: {args.Data}")
        
        let p = new Process(StartInfo = procStartInfo)

        p.OutputDataReceived.AddHandler(DataReceivedEventHandler outputHandler)
        p.ErrorDataReceived.AddHandler(DataReceivedEventHandler errorHandler)
        p.Start() |> ignore
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.WaitForExit()
        outputs

    /// Executes Git command.
    let executeGitCommand (repoDir : string) (command : string) =
        
        executeGitCommandWithResponse repoDir command |> ignore

    let formatRepoString username pass (url : string) = 
        let comb = username + ":" + pass + "@"
        url.Replace("https://","https://" + comb)

    let setLocalEmail (dir : string) (email : string) =
        executeGitCommand dir (sprintf "config user.email \"%s\"" email)

    let tryGetLocalEmail (dir : string) =
        let r = executeGitCommandWithResponse dir "config --local --get user.email"
        if r.Count = 0 then None
        else Some r.[0]

    let setGlobalEmail (email : string) =
        executeGitCommand "" (sprintf "config --global user.email \"%s\"" email)

    let tryGetGlobalEmail () =
        let r = executeGitCommandWithResponse "" "config --global --get user.email"
        if r.Count = 0 then None
        else Some r.[0]

    let setLocalName (dir : string) (name : string) =
        executeGitCommand dir (sprintf "config user.name \"%s\"" name)

    let tryGetLocalName (dir : string) =
        let r = executeGitCommandWithResponse dir "config --local --get user.name"
        if r.Count = 0 then None
        else Some r.[0]

    let setGlobalName (name : string) =
        executeGitCommand "" (sprintf "config --global user.name \"%s\"" name)

    let tryGetGlobalName () =
        let r = executeGitCommandWithResponse "" "config --global --get user.name"
        if r.Count = 0 then None
        else Some r.[0]

    let clone dir url =
        executeGitCommand dir (sprintf "clone %s" url)

    let add dir = 
        executeGitCommand dir "add ."

    let commit dir message =
        executeGitCommand dir (sprintf "commit -m \"%s\"" message)

    let push dir =
        executeGitCommand dir "push"

    /// Stores git credentials to a git host using the git credential interface
    let storeCredentials (log : NLog.Logger) (host : string) username password =

        log.Info($"INFO: Start git credential storing")

        let protocol = "https"
        let host = host.Replace($"{protocol}://","")
        let path = $"git:{protocol}://{host}"
    
        let procStartInfo = 
            ProcessStartInfo(
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                // Redirect standard input, as input is required after the process starts
                RedirectStandardInput = true,
                UseShellExecute = false,
                FileName = "git",
                Arguments = "credential approve"
            )
            
        let outputs = System.Collections.Generic.List<string>()
        let errors = System.Collections.Generic.List<string>()
        let outputHandler (_sender:obj) (args:DataReceivedEventArgs) = 
            outputs.Add args.Data
            log.Trace($"{args.Data}")
        
        let errorHandler (_sender:obj) (args:DataReceivedEventArgs) = 
            
            try

                if args.Data.ToLower().Contains "error" then
                    errors.Add args.Data
                    log.Error($"{args.Data}")
                else 
                    outputs.Add args.Data
                    log.Trace($"{args.Data}")

            with
            | err -> 
                if err.Message.Contains "Object reference not set to an instance of an object" |> not then
                    log.Error($"{err}")

        let p = new Process(StartInfo = procStartInfo)
        
        p.OutputDataReceived.AddHandler(DataReceivedEventHandler outputHandler)
        p.ErrorDataReceived.AddHandler(DataReceivedEventHandler errorHandler)

        log.Trace($"Start storing git credentials by running \"git credential approve\"")

        p.Start() |> ignore
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        
        log.Trace($"Start feeding credentials into git credential interface")

        log.Trace($"url={path}")
        p.StandardInput.WriteLine $"url={path}"
        log.Trace($"username={username}")
        p.StandardInput.WriteLine $"username={username}"
        log.Trace($"host={host}")
        p.StandardInput.WriteLine $"host={host}"
        log.Trace($"path={path}")
        p.StandardInput.WriteLine $"path={path}"
        log.Trace($"protocol={protocol}")
        p.StandardInput.WriteLine $"protocol={protocol}"
        log.Trace($"password={password}")
        p.StandardInput.WriteLine $"password={password}"
        p.StandardInput.WriteLine ""

        log.Trace($"Exiting git credential storing")

        p.WaitForExit()

        errors.Count = 0
            