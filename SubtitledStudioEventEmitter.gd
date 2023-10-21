extends StudioEventEmitter3D

var n

func _ready():
	n = get_node("Subtitle") as Object

func play_subtitled():
	play()
	n.StartSubtitle()
