echo "Splitting $1 into $2 frames"
for ((i = 0; i < $2; i++))
  do
	magick "$1.gif[$i]" $1000$i.png
  done
