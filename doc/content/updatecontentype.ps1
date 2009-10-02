ls -i *.html -R | %{svn propset svn:mime-type text/html $($_.FullName)}
