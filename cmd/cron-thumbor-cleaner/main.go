package main

import (
	"fmt"
	"log"
	"os"
	"os/exec"
	"strconv"
	"time"
)

var (
	logger         = log.New(os.Stderr, time.Now().Format("[2006-01-02 15:04:05] "), 0)
	HourCount      = GetenvOrDefault("HOUR_COUNT", "6")
	PathTempFolder = GetenvOrDefault("PATH_TEMP_FOLDER", "/tmp")
	GlobFilename   = GetenvOrDefault("GLOB_FILENAME", "tmp*")
)

func main() {
	hourCount, err := strconv.Atoi(HourCount)
	if err != nil {
		logger.Fatalln(err)
	}

	for {
		command := fmt.Sprintf(
			"find %s -maxdepth 1 -name \"%s\" -mmin +%d -type f -delete",
			PathTempFolder, GlobFilename, hourCount*60,
		)
		logger.Println(fmt.Sprintf("Executing command: %s", command))
		err = OSExec(command)
		if err != nil {
			logger.Fatalln(err)
		}

		logger.Println(fmt.Sprintf("Waiting for next %d hours", hourCount))
		time.Sleep(time.Duration(hourCount) * time.Hour)
	}
}

func OSExec(command string) (err error) {
	cmd := exec.Command("sh", "-c", command)
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr

	err = cmd.Run()
	if err != nil {
		return err
	}
	return nil
}

func GetenvOrDefault(key string, defaultval string) string {
	if env := os.Getenv(key); env != "" {
		return env
	} else {
		return defaultval
	}
}
