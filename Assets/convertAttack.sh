echo "Splitting $1 into $2 frames"
identify $1.gif
read numFrames
for ((i = 0; i < $numFrames; i++))
  do
	magick "$1.gif[$i]" $1000$i.png
  done
